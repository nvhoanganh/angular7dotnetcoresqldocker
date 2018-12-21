using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetcore.webapi.Models
{
	public class UserDto
	{
		public string Id { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
