using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using AppsManager.DTO;
using Cars.DTO;
using Newtonsoft.Json;

namespace Cars.REST.Client
{
    public class Client
    {
#if DEBUG
        private const string baseUrl = "http://devserver:8081/";
#else
        private const string baseUrl = "http://localhost:8081/";
#endif
        #region Singleton
        private static Client singleton;
        public static Client Instance {
            get {
                if (singleton == null) singleton = new Client();
                return singleton;
            }
        }
        #endregion

        #region Users
        public async Task<List<UserDTO>> GetAllUsers() {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<UserDTO> users = null;
                HttpResponseMessage response = await client.GetAsync("api/v1/users");
                if (response.IsSuccessStatusCode) {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<UserDTO>>(jsonData);
                }

                return users;
            }
        }
        public async Task<List<UserDTO>> GetAgencyUsers(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<UserDTO> users = null;
                HttpResponseMessage response = await client.GetAsync("api/v1/agencies/" + AgencyNumber + "/users");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<UserDTO>>(jsonData);
                }

                return users;
            }
        }
        public async Task<UserDTO> GetUser(string Username)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                UserDTO user = null;
                HttpResponseMessage response = await client.GetAsync("api/v1/users/" + Username);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserDTO>(jsonData);
                }

                return user;
            }
        }
        public async Task<List<UserAgencyDTO>> GetUserAgencies(string Username) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<UserAgencyDTO> agencies = new List<UserAgencyDTO>();
                HttpResponseMessage response = await client.GetAsync("api/v1/users/" + Username + "/agencies");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    List<UserAgencyDTO> entities = JsonConvert.DeserializeObject<List<UserAgencyDTO>>(jsonData);
                    foreach (UserAgencyDTO entity in entities)
                        agencies.Add(entity);
                }

                return agencies;
            }
        }
        public async Task<List<PrivilegeDTO>> GetUserPrivileges(string AgencyNumber, string Username)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<PrivilegeDTO> entities = new List<PrivilegeDTO>();
                HttpResponseMessage response = await client.GetAsync("api/v1/agencies/" + AgencyNumber + "/users/" + Username + "/privileges");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<PrivilegeDTO>>(jsonData);
                }

                return entities;
            }
        }
        public async Task<string> GetUserPrivilegesString(string AgencyNumber, string Username)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string privileges = string.Empty;
                HttpResponseMessage response = await client.GetAsync("api/v1/agencies/" + AgencyNumber + "/users/" + Username + "/privileges");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    List<PrivilegeDTO> entities = JsonConvert.DeserializeObject<List<PrivilegeDTO>>(jsonData);
                    foreach (PrivilegeDTO entity in entities)
                        privileges += entity.Name + ";";
                    if (privileges.Length != 0) privileges = privileges.Substring(0, privileges.Length - 1);
                }

                return privileges;
            }
        }
        public async Task<bool> UpdateUser(string AgencyNumber, UserDTO dto) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/users/" + dto.Username;
                var jsonData = JsonConvert.SerializeObject(dto);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }
        #endregion

        #region Agencies
        public async Task<AgencyDTO> GetAgency(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                AgencyDTO agency = null;
                HttpResponseMessage response = await client.GetAsync("api/v1/agencies/" + AgencyNumber);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    agency = JsonConvert.DeserializeObject<AgencyDTO>(jsonData);
                }

                return agency;
            }
        }
        #endregion

        #region CarReservations
        public async Task<KeyValuePair<int, List<CarReservationDTO>>> GetCarReservations(string AgencyNumber, string Type, int PageSize, int PageNumber, string SearchFor, string Filter) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                KeyValuePair<int, List<CarReservationDTO>> reservations = new KeyValuePair<int, List<CarReservationDTO>>(0, new List<CarReservationDTO>());
                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations?type=" + Type + "&pagesize=" + PageSize + "&pagenumber=" + PageNumber + "&searchfor=" + SearchFor + "&filter=" + Filter;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    reservations = JsonConvert.DeserializeObject<KeyValuePair<int, List<CarReservationDTO>>>(jsonData);
                }

                return reservations;
            }
        }
        public async Task<KeyValuePair<bool, string>> UpdateCarReservation(string AgencyNumber, CarReservationDTO CarReservation) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/" + CarReservation.Id;
                var jsonData = JsonConvert.SerializeObject(CarReservation);
                using (HttpContent content = new StringContent(jsonData)) {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    var rJsonData = response.Content.ReadAsStringAsync().Result;

                    return new KeyValuePair<bool, string>(response.IsSuccessStatusCode, rJsonData);
                }
            }
        }
        public async Task<CarReservationDTO> GetCarReservationById(string AgencyNumber, long Id) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                CarReservationDTO reservation = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/" + Id;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    reservation = JsonConvert.DeserializeObject<CarReservationDTO>(jsonData);
                }

                return reservation;
            }
        }
        public async Task<int> AddCarReservation(string AgencyNumber, CarReservationDTO dto) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/";
                var jsonData = JsonConvert.SerializeObject(dto);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultJsonData = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<int>(resultJsonData);
                    }
                    return -1;
                }
            }
        }
        #endregion

        #region CarCategories
        public async Task<List<CarCategoryDTO>> GetCarCategories(string AgencyNumber) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<CarCategoryDTO> categories = new List<CarCategoryDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/carcategories";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    categories = JsonConvert.DeserializeObject<List<CarCategoryDTO>>(jsonData);
                }

                return categories;
            }
        }
        public async Task<List<CarCategoryDTO>> GetCarCategories(string AgencyNumber, int CarProviderId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<CarCategoryDTO> categories = new List<CarCategoryDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/carcategories?carproviderid=" + CarProviderId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    categories = JsonConvert.DeserializeObject<List<CarCategoryDTO>>(jsonData);
                }

                return categories;
            }
        }
        public async Task<CarCategoryDTO> GetCarCategoryById(string AgencyNumber, int CarCategoryId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                CarCategoryDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/carcategories/" + CarCategoryId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<CarCategoryDTO>(jsonData);
                }

                return entity;
            }
        }
        #endregion

        #region CarProviders
        public async Task<List<CarProviderDTO>> GetCarProviders(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<CarProviderDTO> providers = new List<CarProviderDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/carproviders";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    providers = JsonConvert.DeserializeObject<List<CarProviderDTO>>(jsonData);
                }

                return providers;
            }
        }
        #endregion

        #region TourOperators
        public async Task<List<TourOperatorDTO>> GetTourOperators(string AgencyNumber, bool OnlyActives)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<TourOperatorDTO> operators = new List<TourOperatorDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/touroperators?onlyactives=" + OnlyActives;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    operators = JsonConvert.DeserializeObject<List<TourOperatorDTO>>(jsonData);
                }

                return operators;
            }
        }
        public async Task<List<TourOperatorDTO>> GetTourOperators(string AgencyNumber, int CarProviderId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<TourOperatorDTO> operators = new List<TourOperatorDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/touroperators?carproviderid=" + CarProviderId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    operators = JsonConvert.DeserializeObject<List<TourOperatorDTO>>(jsonData);
                }

                return operators;
            }
        }
        public async Task<TourOperatorDTO> GetTourOperatorById(string AgencyNumber, int TourOperatorId) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                TourOperatorDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/touroperators/" + TourOperatorId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<TourOperatorDTO>(jsonData);
                }

                return entity;
            }
        }
        public async Task<List<CarCategoryDTO>> GetTourOperatorCarCategories(string AgencyNumber, int TourOperatorId) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<CarCategoryDTO> entities = new List<CarCategoryDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/touroperators/" + TourOperatorId + "/carcategories";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<CarCategoryDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region Provinces
        public async Task<List<ProvinceDTO>> GetProvinces(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<ProvinceDTO> entities = new List<ProvinceDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/provinces";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<ProvinceDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region RentCarPlaces
        public async Task<List<RentCarPlaceDTO>> GetActiveRentCarPlaces(string AgencyNumber, int ProvinceId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<RentCarPlaceDTO> entities = new List<RentCarPlaceDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/rentcarplace?provinceid=" + ProvinceId + "&onlyactives=true";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<RentCarPlaceDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region PriceConfigurations
        public async Task<PriceConfigurationDTO> GetPriceConfiguration(string AgencyNumber, int CarCategoryId, int TourOperatorId, string FromDate, string ToDate) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                PriceConfigurationDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/priceconfiguration?carcategoryid=" + CarCategoryId + "&touroperatorid=" + TourOperatorId + "&fromdate=" + FromDate + "&todate=" + ToDate;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<PriceConfigurationDTO>(jsonData);
                }

                return entity;
            }
        }
        #endregion

        #region CarReservationPayments
        public async Task<List<CarReservationPaymentDTO>> GetCarReservationPayments(string AgencyNumber, long CarReservationId) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<CarReservationPaymentDTO> entities = new List<CarReservationPaymentDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/" + CarReservationId + "/payments";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<CarReservationPaymentDTO>>(jsonData);
                }

                return entities;
            }
        }
        public async Task<bool> AddCarReservationPayment(string AgencyNumber, long CarReservationId, CarReservationPaymentDTO DTO)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/" + CarReservationId + "/payments";
                var jsonData = JsonConvert.SerializeObject(DTO);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }
        public async Task<bool> RemoveCarReservationPayment(string AgencyNumber, long CarReservationId, int CarReservationPaymentId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/carreservations/" + CarReservationId + "/payments/" + CarReservationPaymentId;
                HttpResponseMessage response = await client.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region PaymentConcepts
        public async Task<List<PaymentConceptDTO>> GetPaymentConcepts(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<PaymentConceptDTO> entities = new List<PaymentConceptDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/paymentconcepts";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<PaymentConceptDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region PaymentMethods
        public async Task<List<PaymentMethodDTO>> GetPaymentMethods(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<PaymentMethodDTO> entities = new List<PaymentMethodDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/paymentmethods";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<PaymentMethodDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region Clients
        public async Task<KeyValuePair<int, List<ClientDTO>>> GetClients(string AgencyNumber, int PageSize, int PageNumber, string SearchFor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                KeyValuePair<int, List<ClientDTO>> clients = new KeyValuePair<int, List<ClientDTO>>(0, new List<ClientDTO>());
                string url = "api/v1/agencies/" + AgencyNumber + "/clients?pagesize=" + PageSize + "&pagenumber=" + PageNumber + "&searchfor=" + SearchFor;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    clients = JsonConvert.DeserializeObject<KeyValuePair<int, List<ClientDTO>>>(jsonData);
                }

                return clients;
            }
        }
        public async Task<ClientDTO> GetClientById(string AgencyNumber, int ClientId) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ClientDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/clients/" + ClientId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<ClientDTO>(jsonData);
                }

                return entity;
            }
        }
        public async Task<ClientStatisticDTO> GetClientStatistics(string AgencyNumber, int ClientId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ClientStatisticDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/clients/" + ClientId + "/statistics";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<ClientStatisticDTO>(jsonData);
                }

                return entity;
            }
        }
        public async Task<List<CarReservationDTO>> GetClientCarReservations(string AgencyNumber, int ClientId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<CarReservationDTO> reservations = new List<CarReservationDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/clients/" + ClientId + "/carreservations";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    reservations = JsonConvert.DeserializeObject<List<CarReservationDTO>>(jsonData);
                }

                return reservations;
            }
        }
        public async Task<bool> UpdateClient(string AgencyNumber, ClientDTO DTO)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/clients/" + DTO.Id;
                var jsonData = JsonConvert.SerializeObject(DTO);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }
        public async Task<int> AddClient(string AgencyNumber, ClientDTO DTO) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/clients";
                var jsonData = JsonConvert.SerializeObject(DTO);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode) {
                        var jsonDataResult = response.Content.ReadAsStringAsync().Result;
                        var id = JsonConvert.DeserializeObject<int>(jsonDataResult);
                        return id;
                    }
                }
                return -1;
            }
        }
        public async Task<bool> RemoveClient(string AgencyNumber, int ClientId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/clients/" + ClientId;
                HttpResponseMessage response = await client.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region LinkedAgencies
        public async Task<List<LinkedAgencyDTO>> GetAllLinkedAgencies(string AgencyNumber) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<LinkedAgencyDTO> entities = new List<LinkedAgencyDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/linkedagencies";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<LinkedAgencyDTO>>(jsonData);
                }

                return entities;
            }
        }
        public async Task<int> AddLinkedAgency(string AgencyNumber, LinkedAgencyDTO DTO) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/linkedagencies";
                var jsonData = JsonConvert.SerializeObject(DTO);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultJsonData = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<int>(resultJsonData);
                    }
                    return -1;
                }
            }
        }
        public async Task<bool> UpdateLinkedAgency(string AgencyNumber, LinkedAgencyDTO DTO)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/linkedagencies";
                var jsonData = JsonConvert.SerializeObject(DTO);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }
        #endregion

        #region Reports
        public async Task<byte[]> GetVoucherReport(string AgencyNumber, string Format, long CarReservationId) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/voucher?format=" + Format + "&parameters=carreservationid=" + CarReservationId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
            //api / v1 / agencies / 0000 / reports / voucher ? format = pdf & parameters = carreservationid = 13149
        }
        public async Task<byte[]> GetPaymentReceiptReport(string AgencyNumber, string Format, long CarReservationId, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/paymentreceipt?format=" + Format + "&parameters=carreservationid=" + CarReservationId + ";printedby=" + PrintedBy;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
            //api / v1 / agencies / 0000 / reports / voucher ? format = pdf & parameters = carreservationid = 13149
        }
        public async Task<byte[]> GetAccountingReport(string AgencyNumber, string Format, DateTime FromDate, DateTime ToDate, int TourOperatorId, int PaymentStatusId, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/accounting?format=" + Format + "&parameters=fromdate=" + FromDate.ToShortDateString() + ";todate=" + ToDate.ToShortDateString() + ";touroperatorid=" + TourOperatorId + ";paymentstatusid=" + PaymentStatusId + ";printedby=" + PrintedBy;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
        }
        public async Task<byte[]> GetAccountingLimitedReport(string AgencyNumber, string Format, DateTime FromDate, DateTime ToDate, int TourOperatorId, int PaymentStatusId, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/accountinglimited?format=" + Format + "&parameters=fromdate=" + FromDate.ToShortDateString() + ";todate=" + ToDate.ToShortDateString() + ";touroperatorid=" + TourOperatorId + ";paymentstatusid=" + PaymentStatusId + ";printedby=" + PrintedBy; 
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
        }
        public async Task<byte[]> GetPaymentTypesReport(string AgencyNumber, string Format, DateTime FromDate, DateTime ToDate, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/paymenttypes?format=" + Format + "&parameters=fromdate=" + FromDate.ToShortDateString() + ";todate=" + ToDate.ToShortDateString() + ";printedby=" + PrintedBy;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
        }
        public async Task<byte[]> GetSalesByAgentReport(string AgencyNumber, string Format, DateTime FromDate, DateTime ToDate, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/agentsales?format=" + Format + "&parameters=fromdate=" + FromDate.ToShortDateString() + ";todate=" + ToDate.ToShortDateString() + ";printedby=" + PrintedBy;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
        }
        public async Task<byte[]> GetReservationsListReport(string AgencyNumber, string Format, DateTime FromDate, DateTime ToDate, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/carreservations?format=" + Format + "&parameters=fromdate=" + FromDate.ToShortDateString() + ";todate=" + ToDate.ToShortDateString() + ";printedby=" + PrintedBy;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
        }
        public async Task<byte[]> GetCarReservationStatusReport(string AgencyNumber, string Format, DateTime FromDate, DateTime ToDate, string CarReservationStatus, string PrintedBy)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/reports/carreservationstatus?format=" + Format + "&parameters=fromdate=" + FromDate.ToShortDateString() + ";todate=" + ToDate.ToShortDateString() + ";status=" + CarReservationStatus + ";printedby=" + PrintedBy;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<byte[]>(jsonData);
                }
            }
            return new byte[] { };
        }
        #endregion

        #region Seasons
        public async Task<List<SeasonDTO>> GetAllSeasons(string AgencyNumber) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<SeasonDTO> entities = new List<SeasonDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/seasons";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<SeasonDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region ReservationDays
        public async Task<List<ReservationDayDTO>> GetAllReservationDays(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<ReservationDayDTO> entities = new List<ReservationDayDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/reservationdays";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<ReservationDayDTO>>(jsonData);
                }

                return entities;
            }
        }
        #endregion

        #region PriceConfiguration
        public async Task<List<PriceConfigurationDTO>> GetPriceConfigurationByTourOperatorAndSeason(string AgencyNumber, int TourOperatorId, int SeasonId) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<PriceConfigurationDTO> entities = new List<PriceConfigurationDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/priceconfigurations?touroperatorid=" + TourOperatorId + "&seasonid=" + SeasonId;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<PriceConfigurationDTO>>(jsonData);
                }

                return entities;
            }
            //api / v1 / agencies /{ agencynumber}/ priceconfiguration"
        }
        public async Task<bool> UpdatePriceConfiguration(string AgencyNumber, int TourOperatorId, int SeasonId, string ModifiedByUsername, List<PriceConfigurationDTO> Configuration)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "api/v1/agencies/" + AgencyNumber + "/priceconfiguration?touroperatorid=" + TourOperatorId + "&seasonid=" + SeasonId + "&modifiedbyuser=" + ModifiedByUsername;
                var jsonData = JsonConvert.SerializeObject(Configuration);
                using (HttpContent content = new StringContent(jsonData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }
        #endregion

        #region PaymentStatuses
        public async Task<List<PaymentStatusDTO>> GetPaymentStatuses(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<PaymentStatusDTO> entities = new List<PaymentStatusDTO>();
                string url = "api/v1/agencies/" + AgencyNumber + "/paymentstatuses";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<PaymentStatusDTO>>(jsonData);
                }
                return entities;
            }
        }
        #endregion

        #region Privileges
        public async Task<List<PrivilegeDTO>> GetAllPrivileges(string AgencyNumber)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<PrivilegeDTO> entities = null;
                HttpResponseMessage response = await client.GetAsync("api/v1/agencies/" + AgencyNumber + "/privileges");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entities = JsonConvert.DeserializeObject<List<PrivilegeDTO>>(jsonData);
                }

                return entities;
            }
        }
        public async Task<bool> SetUserPrivilege(string AgencyNumber, string Username, string PrivilegeName, bool IsSelected)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (IsSelected)
                {
                    UserPrivilegeDTO dto = new UserPrivilegeDTO()
                    {
                        Username = Username,
                        PrivilegeName = PrivilegeName
                    };

                    string url = "api/v1/agencies/" + AgencyNumber + "/users/" + Username + "/privileges";
                    var jsonData = JsonConvert.SerializeObject(dto);
                    using (HttpContent content = new StringContent(jsonData))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        return response.IsSuccessStatusCode;
                    }
                }
                else
                {
                    HttpResponseMessage response = await client.DeleteAsync("api/v1/agencies/" + AgencyNumber + "/users/" + Username + "/privileges/" + PrivilegeName);
                    return response.IsSuccessStatusCode;
                }
            }
        }
        #endregion

        #region Statistics
        public async Task<CarReservationsPerAgentDTO> GetCarReservationsPerAgentStatistics(string AgencyNumber, string Username, int FirstYear, int SecondYear, int Month) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                CarReservationsPerAgentDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/statistics?dataname=carreservationsperagent&parameters=username=" + Username + ";firstyear=" + FirstYear + ";secondyear=" + SecondYear + ";month=" + Month;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<CarReservationsPerAgentDTO>(jsonData);
                }
                return entity;
            }
        }
        public async Task<PaymentsPerPeriodDTO> GetPaymentsPerPeriodStatistics(string AgencyNumber, DateTime FromDate, int TotalDays, int ComparedWithYear)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                PaymentsPerPeriodDTO entity = null;
                string url = "api/v1/agencies/" + AgencyNumber + "/statistics?dataname=paymentsbydate&parameters=fromdate=" + FromDate.ToShortDateString() + ";totaldays=" + TotalDays + ";comparedwithyear=" + ComparedWithYear;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    entity = JsonConvert.DeserializeObject<PaymentsPerPeriodDTO>(jsonData);
                }
                return entity;
            }
        }
        #endregion
    }
}
