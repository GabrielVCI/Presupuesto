using System.Security.Claims;

namespace Presupuesto.Servicios
{
    public interface IServicioUsuarios  
    {
        string ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private HttpContext httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccesor)
        {
            httpContext = httpContextAccesor.HttpContext;
        }

        public string ObtenerUsuarioId()
        {
            if (httpContext.User.Identity.IsAuthenticated) //Si esta autenticado
            {
                var idClaim = httpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault(); //Buscamos claims (informacion del usuario)
                                                                                       //y vemos si tiene el id en esa informacion

                return idClaim.Value;
            }

            else
            {
                throw new Exception("El usuario no esta autenticado");
            }
        }
    }
}
