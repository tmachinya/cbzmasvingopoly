﻿using BillPayments_LookUp_Validation.Models;
using BillPayments_LookUp_Validation.Models.Requests;
using BillPayments_LookUp_Validation.Models.Responses;
using BillPayments_LookUp_Validation.Services;
using Newtonsoft.Json;
using System.Net;
using System.Xml;

namespace BillPayments_LookUp_Validation.ServicesImplement
{
    public class CSTAccountNumberValidation : ICSTAccountNumberValidation
    {
        public string validate_cst_account_number(BillValidation billerVallidation)
        {
            string url = "https://easylearn.co.zw/portal2/api/cbz/getStudent?target=13&studentNo=" + billerVallidation.FieldValue;

            // Create the request object
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Proxy = new WebProxy("192.168.4.7:80");

            try
            {
                // Get the response from the server
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Read the response stream
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream);
                        string responseJson = reader.ReadToEnd();
                        int statusCode = (int)response.StatusCode;
                        // Handle the response data
                        //Console.WriteLine("Response: " + responseJson);

                        if (!responseJson.Contains("StudentNo"))
                        {
                            BILLVALIDATION bILLVALIDATION = new BILLVALIDATION()
                            {
                                STATUS = new STATUS()
                                {
                                    VALID = "N",
                                    DESC = "Student Number Not Found"

                                },
                                FIELDDESCRIPTION = "",
                                FIELDVALUE = ""
                            };

                            // Stringify the response object to XML string

                            string data_jsonObject = JsonConvert.SerializeObject(bILLVALIDATION);
                            var docBillerValidationResponse = JsonConvert.DeserializeXmlNode(data_jsonObject, "BILL_VALIDATION");

                            using (var stringWriter = new StringWriter())
                            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                            {
                                docBillerValidationResponse!.WriteTo(xmlTextWriter);
                                xmlTextWriter.Flush();
                                string xmloutput = stringWriter.GetStringBuilder().ToString();
                                return xmloutput;
                            };
                        }
                        else
                        {

                            MasvingoPolyCollegeSuccess myApiResponse = JsonConvert.DeserializeObject<MasvingoPolyCollegeSuccess>(responseJson)!;

                            GetStudentByIdRequest getStudentById = new()
                            {
                                FielD_NAME = "UZ_STUDENT_REG",
                                Lov = "H220193A_test"
                            };

                           

                            // Prepare the MFS/Mobile app developers response object

                            BILLVALIDATION bILLVALIDATION = new BILLVALIDATION()
                            {
                                STATUS = new STATUS()
                                {
                                    VALID = "Y",
                                    DESC = "Account is valid"

                                },
                                FIELDDESCRIPTION = myApiResponse.Name,
                                FIELDVALUE = myApiResponse.StudentNo
                            };

                            // Stringify the response object to XML string

                            string data_jsonObject = JsonConvert.SerializeObject(bILLVALIDATION);
                            var docBillerValidationResponse = JsonConvert.DeserializeXmlNode(data_jsonObject, "BILL_VALIDATION");

                            using (var stringWriter = new StringWriter())
                            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                            {
                                docBillerValidationResponse!.WriteTo(xmlTextWriter);
                                xmlTextWriter.Flush();
                                string xmloutput = stringWriter.GetStringBuilder().ToString();
                                return xmloutput;
                            };
                        }

                        return responseJson;
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle any exceptions that occur during the request
                // Console.WriteLine("An error occurred: " + ex.Message);

                // Prepare the MFS/Mobile app developers response object

                BILLVALIDATION bILLVALIDATION = new BILLVALIDATION()
                {
                    STATUS = new STATUS()
                    {
                        VALID = "N",
                        DESC = ex.Message

                    },
                    FIELDDESCRIPTION = "",
                    FIELDVALUE = ""
                };

                // Stringify the response object to XML string

                string data_jsonObject = JsonConvert.SerializeObject(bILLVALIDATION);
                var docBillerValidationResponse = JsonConvert.DeserializeXmlNode(data_jsonObject, "BILL_VALIDATION");

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    docBillerValidationResponse!.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    string xmloutput = stringWriter.GetStringBuilder().ToString();
                    return xmloutput;
                };

            }

            return "first test";
        }
    }
 }
