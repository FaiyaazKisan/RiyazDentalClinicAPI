using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RiyazDentalClinicAPI.Data;
using RiyazDentalClinicAPI.Models;
using RiyazDentalClinicAPI.Models.Dto;
using RiyazDentalClinicAPI.Services;
using RiyazDentalClinicAPI.Utility;
using System.Net;

namespace RiyazDentalClinicAPI.Controllers
{
    [Route("api/MyCases")]
    [ApiController]
    public class MyCaseController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private readonly IWebHostEnvironment _hostEnvironment;
        private SaveImageService _saveImageService;
        public MyCaseController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db=db;
            _response=new ApiResponse();
            _hostEnvironment = hostEnvironment;
            _saveImageService = new SaveImageService();
        }
        [HttpGet]
        public IActionResult GetMyCases()
        {
           _response.Result = _db.MyCases;
           _response.IsSuccess = true;
           _response.StatusCode=HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetMyCases")]
        public IActionResult GetMyCase(int id)
        {
            MyCase myCase = _db.MyCases.FirstOrDefault(u => u.Id == id);
            if(myCase == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Not found");
                return NotFound(_response);
            }
            else
            {
                _response.Result=myCase;
                _response.IsSuccess=true;
                _response.StatusCode=HttpStatusCode.OK;
                return Ok(_response);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostMyCase([FromForm] MyCaseCreateDto myCaseCreateDto)
        {
            try
            { 
                if (ModelState.IsValid)
                {
                    MyCase myCase = new()
                    {
                        Heading = myCaseCreateDto.Heading,
                        File = await _saveImageService.SaveImage(myCaseCreateDto.File, SD.MCA ,_hostEnvironment),
                    };
                    if (myCase.File == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode=HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Incorrect web page");
                        return BadRequest(_response);
                    }
                    _db.MyCases.Add(myCase);
                    _db.SaveChanges();
                    _response.Result = myCase;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.IsSuccess = true;
                    return CreatedAtRoute("GetMyCases", new {id=myCase.Id}, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
            }
            catch(Exception ex)
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
        public async Task<IActionResult> UpdateMyCase(int id, [FromForm] MyCaseUpdateDto myCaseUpdateDto)
        {
            if(id!= myCaseUpdateDto.Id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            MyCase myCase = _db.MyCases.FirstOrDefault(u => u.Id == id);
            if (myCase == null)
            {
                _response.IsSuccess=false;
                _response.StatusCode=HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Case not found");
                return NotFound(_response);
            }
            myCase.Heading = myCaseUpdateDto.Heading;
            if (myCaseUpdateDto.File!=null)
            {
                var oldImageName = myCase.File;
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, oldImageName);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                myCase.File = await _saveImageService.SaveImage(myCaseUpdateDto.File, SD.MCA, _hostEnvironment);
            }
            _db.MyCases.Update(myCase);
            _db.SaveChanges();
            _response.Result = myCase;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteMyCase(int id)
        {
            MyCase myCase = _db.MyCases.FirstOrDefault(u => u.Id == id);
            if (myCase == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            var oldImageName = myCase.File;
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, oldImageName);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _db.MyCases.Remove(myCase);
            _db.SaveChanges();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
