using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper.EmailHelper;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.EmailModel;
using CraftiqueBE.Data.Models.WorkshopRegistrationModel;
using CraftiqueBE.Data.ViewModels.WorkshopRegistrationVM;
using CraftiqueBE.Service.Interfaces;

namespace CraftiqueBE.Service.Services
{
	public class WorkshopServices : IWorkshopServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IEmailHelper _emailHelper;

		public WorkshopServices(IUnitOfWork unitOfWork, IMapper mapper, IEmailHelper emailHelper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_emailHelper = emailHelper;
		}

		public async Task<List<WorkshopRegistrationViewModel>> GetAllAsync()
		{
			var list = await _unitOfWork.WorkshopRegistrationRepository
				.GetAllAsync(w => !w.IsDeleted);
			return _mapper.Map<List<WorkshopRegistrationViewModel>>(list);
		}

		public async Task<WorkshopRegistrationViewModel> AddAsync(CreateWorkshopRegistrationModel model)
		{
			var entity = _mapper.Map<WorkshopRegistration>(model);
			await _unitOfWork.WorkshopRegistrationRepository.AddAsync(entity);
			await _unitOfWork.SaveChangesAsync();

			// Gửi mail xác nhận đăng ký (chờ duyệt)
			var emailBody = BuildConfirmationEmailHtml(entity);
			await _emailHelper.SendMailAsync(CancellationToken.None, new EmailRequestModel
			{
				To = entity.Email,
				Subject = $"Xác nhận đăng ký workshop: {entity.WorkshopName}",
				Body = emailBody
			});

			return _mapper.Map<WorkshopRegistrationViewModel>(entity);
		}

		public async Task<bool> ConfirmAsync(int registrationId)
		{
			var reg = await _unitOfWork.WorkshopRegistrationRepository.GetByIdAsync(registrationId);
			if (reg == null || reg.IsDeleted)
				throw new KeyNotFoundException("Người đăng ký không tồn tại.");

			reg.Status = "ĐÃ XÁC NHẬN";
			await _unitOfWork.SaveChangesAsync();

			// Gửi mail xác nhận từ admin
			var emailBody = BuildAdminConfirmationEmailHtml(reg);
			await _emailHelper.SendMailAsync(CancellationToken.None, new EmailRequestModel
			{
				To = reg.Email,
				Subject = $"Xác nhận tham gia workshop: {reg.WorkshopName}",
				Body = emailBody
			});

			return true;
		}

		public async Task SendEmailAsync(int registrationId, string subject, string body)
		{
			var reg = await _unitOfWork.WorkshopRegistrationRepository.GetByIdAsync(registrationId);
			if (reg == null || reg.IsDeleted)
				throw new KeyNotFoundException("Người đăng ký không tồn tại.");

			await _emailHelper.SendMailAsync(CancellationToken.None, new EmailRequestModel
			{
				To = reg.Email,
				Subject = subject,
				Body = body
			});
		}

		public async Task SendEmailBulkAsync(List<int> registrationIds, string subject, string body)
		{
			var list = await _unitOfWork.WorkshopRegistrationRepository
				.GetAllAsync(r => registrationIds.Contains(r.Id) && !r.IsDeleted);

			foreach (var reg in list)
			{
				await _emailHelper.SendMailAsync(CancellationToken.None, new EmailRequestModel
				{
					To = reg.Email,
					Subject = subject,
					Body = body
				});
			}
		}
		public async Task<bool> RejectAsync(int registrationId, string reason)
		{
			var reg = await _unitOfWork.WorkshopRegistrationRepository.GetByIdAsync(registrationId);
			if (reg == null || reg.IsDeleted)
				throw new KeyNotFoundException("Người đăng ký không tồn tại.");

			if (reg.Status == "ĐÃ TỪ CHỐI")
				return true;

			reg.Status = "ĐÃ TỪ CHỐI";
			await _unitOfWork.SaveChangesAsync();

			// Gửi email thông báo từ chối
			var emailBody = BuildRejectionEmailHtml(reg, reason);
			await _emailHelper.SendMailAsync(CancellationToken.None, new EmailRequestModel
			{
				To = reg.Email,
				Subject = $"Thông báo từ chối đăng ký workshop: {reg.WorkshopName}",
				Body = emailBody
			});

			return true;
		}

		private string BuildConfirmationEmailHtml(WorkshopRegistration reg)
		{
			return $@"
			<html>
			<head><meta charset='UTF-8'></head>
			<body style='font-family: Arial, sans-serif;'>
			  <div style='padding: 20px; max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px;'>
				<h2 style='color: green;'>🎉 Đăng ký workshop thành công!</h2>
				<p>Xin chào <strong>{reg.FullName}</strong>,</p>
				<p>Cảm ơn bạn đã đăng ký tham gia <strong>workshop: {reg.WorkshopName}</strong>.</p>
				<p>Thông tin sẽ được gửi thêm sau khi xác nhận.</p>
				<ul>
				  <li><b>📅 Ngày đăng ký:</b> {reg.RegisteredDate:dd/MM/yyyy}</li>
				  <li><b>📞 SĐT:</b> {reg.PhoneNumber}</li>
				  <li><b>📧 Email:</b> {reg.Email}</li>
				  <li><b>📌 Trạng thái:</b> {reg.Status}</li>
				</ul>
				<p>Trân trọng,<br><strong>Đội ngũ Craftique</strong></p>
			  </div>
			</body>
			</html>";
		}

		private string BuildAdminConfirmationEmailHtml(WorkshopRegistration reg)
		{
			return $@"
			<html>
			<head><meta charset='UTF-8'></head>
			<body style='font-family: Arial, sans-serif;'>
			  <div style='padding: 20px; max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px;'>
				<h2 style='color: #007bff;'>✔ Workshop của bạn đã được xác nhận!</h2>
				<p>Xin chào <strong>{reg.FullName}</strong>,</p>
				<p>Đơn đăng ký tham gia workshop: <strong>{reg.WorkshopName}</strong> đã được xác nhận thành công.</p>
				<p>Vui lòng kiểm tra lại thông tin sau:</p>
				<ul>
				  <li><b>📅 Ngày đăng ký:</b> {reg.RegisteredDate:dd/MM/yyyy}</li>
				  <li><b>📞 SĐT:</b> {reg.PhoneNumber}</li>
				  <li><b>📧 Email:</b> {reg.Email}</li>
				  <li><b>📌 Trạng thái:</b> ĐÃ XÁC NHẬN</li>
				</ul>
				<p>Mọi thắc mắc vui lòng liên hệ đội ngũ Craftique.</p>
				<p style='margin-top: 30px;'>Trân trọng,<br><strong>Đội ngũ Craftique</strong></p>
			  </div>
			</body>
			</html>";
		}

		private string BuildRejectionEmailHtml(WorkshopRegistration reg, string reason)
		{
			return $@"
			<html>
			<head><meta charset='UTF-8'></head>
			<body style='font-family: Arial, sans-serif;'>
			  <div style='padding: 20px; max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px;'>
				<h2 style='color: red;'>❌ Đăng ký workshop không được chấp nhận</h2>
				<p>Xin chào <strong>{reg.FullName}</strong>,</p>
				<p>Chúng tôi rất tiếc phải thông báo rằng đơn đăng ký tham gia workshop <strong>{reg.WorkshopName}</strong> của bạn đã bị từ chối.</p>
				<p><b>Lý do:</b> {reason}</p>
				<ul>
				  <li><b>📅 Ngày đăng ký:</b> {reg.RegisteredDate:dd/MM/yyyy}</li>
				  <li><b>📞 SĐT:</b> {reg.PhoneNumber}</li>
				  <li><b>📧 Email:</b> {reg.Email}</li>
				  <li><b>📌 Trạng thái:</b> ĐÃ TỪ CHỐI</li>
				</ul>
				<p>Mọi thắc mắc vui lòng liên hệ đội ngũ Craftique để biết thêm thông tin.</p>
				<p style='margin-top: 30px;'>Trân trọng,<br><strong>Đội ngũ Craftique</strong></p>
			  </div>
			</body>
			</html>";
		}
	}
}
