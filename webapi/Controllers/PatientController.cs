using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


namespace Benefits.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        public IConfiguration _configuration { get; }

        public PatientController(IConfiguration configuration, 
                                IHttpClientFactory clientFactory, 
                                ILogger<PatientController> logger)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration  = configuration;
        }

        [HttpGet]
        public async Task<string[]> Get()
        {
            var rng = new Random();
            string insuranceServiceResponse = await ProcessPatientBenefits();

            string benefits = "Paid time off, Retirement benefits, Healthcare spending";
            string[] benefitsAndInsurance = { benefits, insuranceServiceResponse };   

            Console.WriteLine("Benefits: " + benefits);
            return benefitsAndInsurance;
        }

        private async Task<string> ProcessPatientBenefits()
        {
            string toReturn = string.Empty;
            try {
                var url = _configuration["InsUrl"] + "/WeatherForecast";
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                var client = _clientFactory.CreateClient("InsuranceClient");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    toReturn = await response.Content.ReadAsStringAsync();    
                }
                else
                {
                    toReturn = response.StatusCode + "" + response.ReasonPhrase;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return ex.ToString();
            }
            return toReturn;
        }
    }
}
