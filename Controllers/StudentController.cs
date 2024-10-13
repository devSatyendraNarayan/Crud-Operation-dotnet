namespace WebApplication1.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using WebApplication1.Models;

    public class StudentController : Controller
    {
        private readonly StudentContexts _Db;

        public StudentController(StudentContexts db)
        {
            _Db = db;
        }


            public IActionResult StudentList()
            {
                return View();
            }

           
      
        [HttpPost]
        public async Task<IActionResult> LoadStudentData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 10;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Base query
                var query = from s in _Db.tbl_student
                            join d in _Db.tbl_departments on s.DepID equals d.ID
                            select new
                            {
                                s.ID,
                                s.Name,
                                s.Email,
                                d.Department,
                                s.Mobile,
                                s.Description
                            };

                // Filter the data
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(s => s.Name.Contains(searchValue) ||
                                             s.Email.Contains(searchValue) ||
                                             s.Department.Contains(searchValue) ||
                                              s.Description.Contains(searchValue) ||
                                             s.Mobile.Contains(searchValue));
                }

                // Sorting logic
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Default ordering by "ID" if no sort column provided 
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection) && sortColumn != "sNo")
                {
                    query = query.OrderBy($"{sortColumn} {sortDirection}");
                }
                else
                {
                    query = query.OrderBy("ID desc"); // Default order by ID if no valid sort column
                }

                // Total records after filtering
                var totalRecords = await query.CountAsync();

                // Apply pagination using Skip and Take after ordering
                var students = await query.Skip(skip).Take(pageSize).ToListAsync();

                // Prepare the data for the DataTables response
                var data = students.Select((s, index) => new
                {
                    sNo = skip + index + 1,  // Serial number calculation
                    s.ID,
                    s.Name,
                    s.Email,
                    Department = s.Department,
                    s.Mobile,
                    s.Description,
                    actions = $"<div class='d-flex justify-content-start'>" +
          $"<a href='/Student/Edit/{s.ID}' class='btn btn-success btn-sm mx-1' title='Edit'>Edit</a>" +
          $"<a href='/Student/Details/{s.ID}' class='btn btn-info btn-sm mx-1' title='Details'>Details</a>" +
          $"<a href='/Student/Delete/{s.ID}' class='btn btn-danger btn-sm mx-1' title='Delete'>Delete</a>" +
          $"</div>"

                }).ToList();

                // Return JSON data for DataTables
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = totalRecords,
                    recordsTotal = totalRecords,
                    data = data
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return Json(new { error = "An error occurred while loading data: " + ex.Message });
            }
        }





        public IActionResult Create()
        {
            loadDepartment();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Students obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _Db.tbl_student.AddAsync(obj); 
                    await _Db.SaveChangesAsync();
                    return Json(new { success = true, message = "Student data created successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error occurred while creating student: " + ex.Message });
                }
            }

            loadDepartment();
            return Json(new { success = false, message = "Invalid data." });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _Db.tbl_student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            loadDepartment();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Students student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _Db.tbl_student.Update(student); 
                    await _Db.SaveChangesAsync();
                    return RedirectToAction("StudentList");
                }
                catch (Exception ex)
                {
                    return View("Error", new { message = "An error occurred while saving the student data: " + ex.Message });
                }
            }

            loadDepartment();
            return View(student);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var student = await (from s in _Db.tbl_student
                                     join d in _Db.tbl_departments on s.DepID equals d.ID 
                                     where s.ID == id 
                                     select new Students
                                     {
                                         ID = s.ID,
                                         Name = s.Name,
                                         Mobile = s.Mobile,
                                         Email = s.Email,
                                         DepID = s.DepID,
                                         Description = s.Description,
                                         Department = d.Department 
                                     })
                                      .FirstOrDefaultAsync();

                if (student == null)
                {
                    return Json(new { success = false, message = "Student data not found." });
                }

                return View(student);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while fetching student details: " + ex.Message });
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await (from s in _Db.tbl_student
                                 join d in _Db.tbl_departments on s.DepID equals d.ID
                                 where s.ID == id
                                 select new Students
                                 {
                                     ID = s.ID,
                                     Name = s.Name,
                                     Mobile = s.Mobile,
                                     Email = s.Email,
                                     DepID = s.DepID,
                                     Description = s.Description,
                                     Department = d.Department // Fetch department name
                                 }).FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _Db.tbl_student.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            try
            {
                _Db.tbl_student.Remove(student);
                await _Db.SaveChangesAsync();

                return Json(new { success = true, message = "Student data deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred while deleting student: " + ex.Message });
            }
        }

        private void loadDepartment()
        {
            try
            {
                ViewBag.DepList = _Db.tbl_departments.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.DepList = new List<Departments> { new Departments { ID = 0, Department = "Error loading departments: " + ex.Message } };
            }
        }
    }
}
