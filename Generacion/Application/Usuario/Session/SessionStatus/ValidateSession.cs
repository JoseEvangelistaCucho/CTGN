using Generacion.Application.Usuario.Session.Command;
using Generacion.Models.Usuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Net.NetworkInformation;

namespace Generacion.Application.Usuario.Session.SessionStatus
{
    public class ValidarSesion : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            GuardarEnSesion(context.HttpContext, context);

            if (!context.HttpContext.Session.TryGetValue("usuarioDetail", out byte[] usuarioBytes))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
            else
            {
                string usuarioDetail = context.HttpContext.Session.GetString("usuarioDetail");
                DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

                if (string.IsNullOrEmpty(user.Nombre) && string.IsNullOrEmpty(user.Apellidos))
                {
                    context.Result = new RedirectToActionResult("Index", "Login", null);
                }
            }
            base.OnActionExecuting(context);
        }

        private async void GuardarEnSesion(HttpContext context, ActionExecutingContext filterContext)
        {
            string rutaAccion = filterContext.ActionDescriptor.DisplayName; ;

            var iPAddress = context.Connection.RemoteIpAddress;
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            string detalleDeDispositivo =$"iP Address: {iPAddress}  <<>>   macAddr: {macAddr} ";

            var command = new GuardarSessionUsuario
            {
                Detalle = detalleDeDispositivo,
                RutaAccion = rutaAccion
            };

            var mediator = context.RequestServices.GetRequiredService<IMediator>();
            await mediator.Send(command);
        }
    }
}
