using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RxSubstitution.Controllers;
using RxSubstitution.Models;

namespace RxSubstitution.Test
{
    [TestClass]
    public class SubstitutionControllerTests
    {
        [TestMethod]
        public void TestGetRxList()
        {
            //set up
            SubstitutionController c = new SubstitutionController();
            string serviceURL = "http://api-sandbox.pillpack.com/prescriptions";
            //execute
            List<RxModel> prescriptions = c.GetRxList(serviceURL);
            //test
            Assert.IsTrue(prescriptions.Count > 0);
        }


        [TestMethod]
        public void TestGetDrugList()
        {
            //set up
            SubstitutionController c = new SubstitutionController();
            string serviceURL = "http://api-sandbox.pillpack.com/medications";
            string drugId = "564aab6f3032360003010000"; // Bayer 10mg
            //execute
            List<DrugModel> drugs = c.GetDrugOptions(serviceURL, drugId);
            //test
            Assert.IsTrue(drugs.Count > 0);
        }


        [TestMethod]
        public void TestGenerateSubstitutionList()
        {
            //set up
            SubstitutionController c = new SubstitutionController();
            string serviceRootURL = "http://api-sandbox.pillpack.com/";
            List<RxModel> prescriptions = c.GetRxList(serviceRootURL + "prescriptions");
            List<RxModel> subsetOfPrescriptions = prescriptions.GetRange(1, 10); //for testing purposes, only do the first 10.
            //execute
            List<RxUpdateModel> updates = c.GenerateRxSubstituionList(serviceRootURL, subsetOfPrescriptions);
            //test
            Assert.IsTrue(updates.Count > 0);
        }

    }
}
