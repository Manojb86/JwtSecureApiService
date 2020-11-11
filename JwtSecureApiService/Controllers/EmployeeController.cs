using JwtSecureApiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace JwtSecureApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private List<Employee> employees;

        public EmployeeController()
        {
            SetupEmployeeList();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return employees;
        }

        private void SetupEmployeeList()
        {
            employees = new List<Employee> {
                new Employee{ Id = 1, Name = "Manoj", Address = "Colombo, Sri Lanka", DepartmentName = "Development" },
                new Employee{ Id = 2, Name = "Manee", Address = "Colombo, Sri Lanka", DepartmentName = "Marketing" },
                new Employee{ Id = 3, Name = "Malin", Address = "Colombo, Sri Lanka", DepartmentName = "Development" },
                new Employee{ Id = 4, Name = "Harshi", Address = "Colombo, Sri Lanka", DepartmentName = "HR" },
                new Employee{ Id = 5, Name = "Mahinda", Address = "Colombo, Sri Lanka", DepartmentName = "Finance" }
            };
        }
    }
}