using Newtonsoft.Json;
using RxSubstitution.Controllers;
using RxSubstitution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSubstitution
{
    class Program
    {
        static void Main(string[] args)
        {
            //I chose to write this as a console app because it was both simple (as in the exercise wouldn't get lost in the boilerplate of the project)
            // and because console apps can very practically be converted into azure web jobs, which would be a likely method to employ this code.

            if (args.Length < 1) Console.WriteLine("You must pass the root service URL as a parameter to this exe");
            else
            {
                string rootServiceURL = args[0];
                SubstitutionController c = new SubstitutionController();
                List<RxModel> prescriptions = c.GetRxList(rootServiceURL + "prescriptions");
                //We may want to limit the number we process at any given time.
                //List<RxModel> subsetOfPrescriptions = prescriptions.GetRange(1, 10); 
                List<RxUpdateModel> updates = c.GenerateRxSubstituionList(rootServiceURL, prescriptions);

                //convert the result set to a string
                string jsonResult = JsonConvert.SerializeObject(updates);

                //the secnario wasn't precisely clear how the output was to be formatted 
                // (written to a file? submitted to another service?), so for now I'm outputting it to the console.
                Console.Write(jsonResult);
            }
            Console.ReadKey();
        }
    }
}
