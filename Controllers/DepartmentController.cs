using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly StudentContexts _Db;
        public DepartmentController(StudentContexts db)
        {
            _Db = db;
        }

        public IActionResult DepartList()
        {
            var departments=_Db.tbl_departments.ToList();
            return View(departments);
        }

        public IActionResult DepartCreate()
        {
            return View();
        }

        [HttpPost]


        public IActionResult DepartCreate(Departments departments)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _Db.tbl_departments.Add(departments);
                    _Db.SaveChanges();
                    return Json(new { success = true, message = "Department created successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error occurred while creating department." });
                }
            }
            return Json(new { success = false, message = "Invalid data." });
        }
        public IActionResult DepartEdit(int id)
        {
            var department = _Db.tbl_departments.FirstOrDefault(d => d.ID == id);
            if (department == null)
            {
                return Json(new { success = false, message = "Department not found." });
            }
            //return Json(new { success = true, data = department });
            return View(department);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult DepartEdit(int id, Departments departments)
        {
            if (id != departments.ID)
            {
                return Json(new { success = false, message = "Bad Request." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _Db.tbl_departments.Update(departments);
                    _Db.SaveChanges();
                    //return Json(new { success = true, message = "Department updated successfully." });
                    return Json(new { success = true, data = new { id = departments.ID, department = departments.Department } });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error occurred while updating department." });
                }
            }
            return Json(new { success = false, message = "Invalid data." });
        }


        public IActionResult DepartDetails(int id)
        {
            var department = _Db.tbl_departments.FirstOrDefault(d => d.ID == id);
            if (department == null)
            {
                return Json(new { success = false, message = "Department not found." });
            }
            //return Json(new { success = true, data = department });
            return View(department);
        }

        public IActionResult DepartDelete(int id)
        {
            var department = _Db.tbl_departments.FirstOrDefault(d => d.ID == id);
            if(department==null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        public IActionResult DepartRemove(int id)
        {
            try
            {
                var department = _Db.tbl_departments.Find(id);
                if (department == null)
                {
                    return Json(new { success = false, message = "Department not found." });
                }

                _Db.tbl_departments.Remove(department);
                _Db.SaveChanges();

                return Json(new { success = true, message = "Department deleted successfully." });
            }
            catch (Exception ex)
            {
              
                return Json(new { success = false, message = "An error occurred while trying to delete the department." });
            }
        }



    }
}
