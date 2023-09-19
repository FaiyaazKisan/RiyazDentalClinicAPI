using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiyazDentalClinicAPI.Data;
using RiyazDentalClinicAPI.Models.Dto;
using RiyazDentalClinicAPI.Models;
using RiyazDentalClinicAPI.Services;
using RiyazDentalClinicAPI.Utility;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace RiyazDentalClinicAPI.Controllers
{
    [Route("api/PatientsTestimonials")]
    [ApiController]
    public class PatientsTestimonialController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private readonly IWebHostEnvironment _hostEnvironment;
        private SaveImageService _saveImageService;
        public PatientsTestimonialController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _response = new ApiResponse();
            _hostEnvironment = hostEnvironment;
            _saveImageService = new SaveImageService();
        }
        [HttpGet]
        public IActionResult GetPatientsTestimonials()
        {
            _response.Result = _db.PatientsTestimonials;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetPatientsTestimonials")]
        public IActionResult GetPatientsTestimonial(int id)
        {
            PatientsTestimonial patientsTestimonial = _db.PatientsTestimonials.FirstOrDefault(u => u.Id == id);
            if (patientsTestimonial == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Not found");
                return NotFound(_response);
            }
            else
            {
                _response.Result = patientsTestimonial;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostPatientsTestimonial([FromForm] PatientsTestimonialCreateDto PatientsTestimonialCreateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PatientsTestimonial patientsTestimonial = new()
                    {
                        Heading = PatientsTestimonialCreateDto.Heading,
                        File = await _saveImageService.SaveImage(PatientsTestimonialCreateDto.File, SD.PTA, _hostEnvironment),
                    };
                    if (patientsTestimonial.File == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Incorrect web page");
                        return BadRequest(_response);
                    }
                    _db.PatientsTestimonials.Add(patientsTestimonial);
                    _db.SaveChanges();
                    _response.Result = patientsTestimonial;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.IsSuccess = true;
                    return CreatedAtRoute("GetPatientsTestimonials", new { id = patientsTestimonial.Id }, _response);
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
        public async Task<IActionResult> UpdatePatientsTestimonial(int id, [FromForm] PatientsTestimonialUpdateDto patientsTestimonialUpdateDto)
        {
            if (id != patientsTestimonialUpdateDto.Id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            PatientsTestimonial patientsTestimonial = _db.PatientsTestimonials.FirstOrDefault(u => u.Id == id);
            if (patientsTestimonial == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Case not found");
                return NotFound(_response);
            }
            patientsTestimonial.Heading = patientsTestimonialUpdateDto.Heading;
            if (patientsTestimonialUpdateDto.File != null)
            {
                var oldImageName = patientsTestimonial.File;
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, oldImageName);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                patientsTestimonial.File = await _saveImageService.SaveImage(patientsTestimonialUpdateDto.File, SD.PTA, _hostEnvironment);
            }
            _db.PatientsTestimonials.Update(patientsTestimonial);
            _db.SaveChanges();
            _response.Result = patientsTestimonial;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult DeletePatientsTestimonial(int id)
        {
            PatientsTestimonial patientsTestimonial = _db.PatientsTestimonials.FirstOrDefault(u => u.Id == id);
            if (patientsTestimonial == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            var oldImageName = patientsTestimonial.File;
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, oldImageName);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _db.PatientsTestimonials.Remove(patientsTestimonial);
            _db.SaveChanges();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
