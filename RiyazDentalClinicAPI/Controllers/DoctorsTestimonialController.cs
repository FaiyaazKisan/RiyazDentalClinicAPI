using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiyazDentalClinicAPI.Data;
using RiyazDentalClinicAPI.Models;
using RiyazDentalClinicAPI.Models.Dto;
using RiyazDentalClinicAPI.Services;
using RiyazDentalClinicAPI.Utility;
using System.Net;

namespace RiyazDentalClinicAPI.Controllers
{
    [Route("api/DoctorsTestimonials")]
    [ApiController]
    public class DoctorsTestimonialController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private readonly IWebHostEnvironment _hostEnvironment;
        private SaveImageService _saveImageService;
        public DoctorsTestimonialController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _response = new ApiResponse();
            _hostEnvironment = hostEnvironment;
            _saveImageService = new SaveImageService();
        }
        [HttpGet]
        public IActionResult GetDoctorsTestimonials()
        {
            _response.Result = _db.DoctorsTestimonials;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetDoctorsTestimonials")]
        public IActionResult GetDoctorsTestimonial(int id)
        {
            DoctorsTestimonial doctorsTestimonial = _db.DoctorsTestimonials.FirstOrDefault(u => u.Id == id);
            if (doctorsTestimonial == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Not found");
                return NotFound(_response);
            }
            else
            {
                _response.Result = doctorsTestimonial;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostDoctorsTestimonial([FromForm] DoctorsTestimonialCreateDto doctorsTestimonialCreateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DoctorsTestimonial doctorsTestimonial = new()
                    {
                        Heading = doctorsTestimonialCreateDto.Heading,
                        File = await _saveImageService.SaveImage(doctorsTestimonialCreateDto.File, SD.DTA, _hostEnvironment),
                    };
                    if (doctorsTestimonial.File == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Incorrect web page");
                        return BadRequest(_response);
                    }
                    _db.DoctorsTestimonials.Add(doctorsTestimonial);
                    _db.SaveChanges();
                    _response.Result = doctorsTestimonial;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.IsSuccess = true;
                    return CreatedAtRoute("GetDoctorsTestimonials", new { id = doctorsTestimonial.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
                return BadRequest(_response);
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDoctorsTestimonial(int id, [FromForm] DoctorsTestimonialUpdateDto doctorsTestimonialUpdateDto)
        {
            if (id != doctorsTestimonialUpdateDto.Id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            DoctorsTestimonial doctorsTestimonial = _db.DoctorsTestimonials.FirstOrDefault(u => u.Id == id);
            if (doctorsTestimonial == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Case not found");
                return NotFound(_response);
            }
            doctorsTestimonial.Heading = doctorsTestimonialUpdateDto.Heading;
            if (doctorsTestimonialUpdateDto.File != null)
            {
                var oldImageName = doctorsTestimonial.File;
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, oldImageName);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                doctorsTestimonial.File = await _saveImageService.SaveImage(doctorsTestimonialUpdateDto.File, SD.DTA, _hostEnvironment);
            }
            _db.DoctorsTestimonials.Update(doctorsTestimonial);
            _db.SaveChanges();
            _response.Result = doctorsTestimonial;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteDoctorsTestimonial(int id)
        {
            DoctorsTestimonial doctorsTestimonial = _db.DoctorsTestimonials.FirstOrDefault(u => u.Id == id);
            if (doctorsTestimonial == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            var oldImageName = doctorsTestimonial.File;
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, oldImageName);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _db.DoctorsTestimonials.Remove(doctorsTestimonial);
            _db.SaveChanges();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
