using Newtonsoft.Json;
using RxSubstitution.Models;
using RxSubstitution.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RxSubstitution.Controllers
{
    public class SubstitutionController
    {

        public List<RxModel> GetRxList(string serviceURL)
        {
            List<RxModel> result = new List<RxModel>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceURL);
            try
            {
                WebResponse response = request.GetResponse();
                string jsonResponse = "";
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    jsonResponse = reader.ReadToEnd();
                }

                result = JsonConvert.DeserializeObject<List<RxModel>>(jsonResponse);
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    //Log Error
                    ErrorLoggingService.Logger.LogError(errorText, ex.StackTrace, serviceURL);
                }
                throw;
            }

            return result;
        }

        public List<DrugModel> GetDrugOptions(string serviceURL, string drugId)
        {
            List<DrugModel> result = new List<DrugModel>();
            DrugModel prescribedDrug = new DrugModel();
            //get the specific drug via id

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceURL + "/" + drugId);
            try
            {
                WebResponse response = request.GetResponse();
                string jsonResponse = "";
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    jsonResponse = reader.ReadToEnd();
                }

                prescribedDrug = JsonConvert.DeserializeObject<DrugModel>(jsonResponse);

            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    //Log Error
                    ErrorLoggingService.Logger.LogError(errorText, ex.StackTrace, serviceURL);
                }
                throw;
            }

            //use the rxcui from the drug model to query for all substitutions
            if (prescribedDrug.rxcui != "")
            {
                request = (HttpWebRequest)WebRequest.Create(serviceURL + "?rxcui=" + prescribedDrug.rxcui);
                try
                {
                    WebResponse response = request.GetResponse();
                    string jsonResponse = "";
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                        jsonResponse = reader.ReadToEnd();
                    }

                    result = JsonConvert.DeserializeObject<List<DrugModel>>(jsonResponse);

                }
                catch (WebException ex)
                {
                    WebResponse errorResponse = ex.Response;
                    using (Stream responseStream = errorResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                        String errorText = reader.ReadToEnd();
                        //Log Error
                        ErrorLoggingService.Logger.LogError(errorText, ex.StackTrace, serviceURL);
                    }
                    throw;
                }
            }
            return result;
        }

        public List<RxUpdateModel> GenerateRxSubstituionList(string rootServiceURL, List<RxModel> rxList)
        {
            List<RxUpdateModel> result = new List<RxUpdateModel>();

            try
            {
                //todo : loop through the rxList, foreach rx
                foreach(RxModel rx in rxList)
                {
                    //get the drug options for that Rx
                    List<DrugModel> options = GetDrugOptions(rootServiceURL + "medications", rx.medication_id);
                    //loop through the options to determine if a generic is available 
                    foreach(DrugModel option in options)
                    {
                        // if (generic == true) then add new RxUpdateModel to the result list with the Rx and the generic's drugid
                        if (option.generic)
                        {
                            RxUpdateModel update = new RxUpdateModel();
                            update.prescription_id = rx.id;
                            update.medication_id = option.id;

                            //check for duplicates in the output (since that might be bad)
                            if (!result.Contains(update)) result.Add(update);
                            //there's nothing in the requirements about how duplicates should be handled.
                            //It's possible that the same Rx could be in the list multiple times (which I've screened for),
                            // it's also possible that multiple alternatives may exist for a specific drug. For the time being, 
                            // I've assumed that this is OK, but this is an issue I'd defintely raise with the product manager to 
                            // make sure was a safe assumption.
                        }
                    }

                    //I imagine we may want to do something here to remove the rx from the queue, so that we don't process it multiple times.
                }

            }
            catch (Exception ex)
            {
                ErrorLoggingService.Logger.LogError(ex.Message, ex.StackTrace, rootServiceURL);
            }
            return result;
        }
    }
}
