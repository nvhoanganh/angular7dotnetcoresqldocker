using dotnetcore.webapi.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dotnetcore.webapi.Models;

namespace dotnetcore.webapi
{
   public class DockerDbContext : IdentityDbContext<ApplicationUser>
   {
      public DockerDbContext(DbContextOptions<DockerDbContext> options)
         : base(options)
      {
      }
   }
}
