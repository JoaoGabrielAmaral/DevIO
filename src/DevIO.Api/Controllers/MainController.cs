using DevIO.Business.Interfaces;
using DevIO.Business.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotifier _notifier;
        public readonly IUser _user;

        protected Guid UsuarioId { get; set; }
        protected bool UsuarioAutenticado { get; set; }

        public MainController(INotifier notifier, IUser user)
        {
            _notifier = notifier;
            _user = user;

            UsuarioId = user.GetUserId();
            UsuarioAutenticado = user.IsAuthenticated();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (_notifier.IsValid())
            {
                return Ok(new
                {
                    Success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notifier.Get().Select(e => e.Message)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) VerifyModelErrors(modelState);

            return CustomResponse();
        }

        protected void VerifyModelErrors(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(m => m.Errors);
            foreach (var error in erros)
            {
                var msg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
                NotifyError(msg);
            }
        }

        protected void NotifyError(string msg)
        {
            _notifier.Handle(new Notification(msg));
        }
    }
}
