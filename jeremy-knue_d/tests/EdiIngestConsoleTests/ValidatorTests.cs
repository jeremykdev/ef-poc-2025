using DotNetEnv;
using EdiFabric.Templates.Hipaa5010;
using EdiIngestConsole.Validations;
using Newtonsoft.Json;

namespace EdiIngestConsoleTests;

[TestFixture]
public class ValidatorTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        // parser depends on key in enviroment variable
        // set path to .env file containing license key
        // also need SNIP_VALIDATION_LEVEL=1 for validation level 2
        Env.Load(@"..\..\..\.env");
    }


    [Test]
    public void Invalid_837P_Should_Return_Valid_False()
    {
        // arrange
        Validator validator = new();
        var ediMessage = this.Get837P();
        // set an invalid code
        ediMessage.BHT_BeginningOfHierarchicalTransaction.HierarchicalStructureCode_01 = "JUNK";  

        // act 
        var result = validator.Validate(ediMessage);

        // assert 
        Assert.That(result.IsValid, Is.False);
    }


    [Test]
    public void Valid_837P_Should_Return_Valid_True()
    {
        // arrange
        Validator validator = new();
        var ediMessage = this.Get837P(); 

        // act 
        var result = validator.Validate(ediMessage);

        // assert 
        Assert.That(result.IsValid, Is.True);
    }


    /// <summary>
    /// Get a valid 837P instance
    /// </summary>
    private TS837P Get837P() =>
        JsonConvert.DeserializeObject<TS837P>(_Json837P)!;        


    // got this by putting a valid 837P into EDI nation 
    // then converted to JSON and got just the one 837P transaction
    // see: https://www.edifabric.com/edination/
    private const string _Json837P = """
    {
            "ST": {
              "TransactionSetIdentifierCode_01": "837",
              "TransactionSetControlNumber_02": "1239",
              "ImplementationConventionPreference_03": "005010X222A1"
            },
            "BHT_BeginningOfHierarchicalTransaction": {
              "HierarchicalStructureCode_01": "0019",
              "TransactionSetPurposeCode_02": "00",
              "SubmitterTransactionIdentifier_03": "010",
              "TransactionSetCreationDate_04": "20170617",
              "TransactionSetCreationTime_05": "1741",
              "TransactionTypeCode_06": "CH"
            },
            "AllNM1": {
              "Loop1000A": {
                "NM1_SubmitterName": {
                  "EntityIdentifierCode_01": "41",
                  "EntityTypeQualifier_02": "2",
                  "ResponseContactLastorOrganizationName_03": "SUBMITTER",
                  "IdentificationCodeQualifier_08": "46",
                  "ResponseContactIdentifier_09": "ABC123"
                },
                "PER_SubmitterEDIContactInformation": [
                  {
                    "ContactFunctionCode_01": "IC",
                    "ResponseContactName_02": "BOB SMITH",
                    "CommunicationNumberQualifier_03": "TE",
                    "ResponseContactCommunicationNumber_04": "4805551212"
                  }
                ]
              },
              "Loop1000B": {
                "NM1_ReceiverName": {
                  "EntityIdentifierCode_01": "40",
                  "EntityTypeQualifier_02": "2",
                  "ResponseContactLastorOrganizationName_03": "RECEIVER",
                  "IdentificationCodeQualifier_08": "46",
                  "ResponseContactIdentifier_09": "44556"
                }
              }
            },
            "Loop2000A": [
              {
                "HL_BillingProviderHierarchicalLevel": {
                  "HierarchicalIDNumber_01": "1",
                  "HierarchicalLevelCode_03": "20",
                  "HierarchicalChildCode_04": "1"
                },
                "AllNM1": {
                  "Loop2010AA": {
                    "NM1_BillingProviderName": {
                      "EntityIdentifierCode_01": "85",
                      "EntityTypeQualifier_02": "2",
                      "ResponseContactLastorOrganizationName_03": "BILLING PROVIDER",
                      "IdentificationCodeQualifier_08": "XX",
                      "ResponseContactIdentifier_09": "1122334455"
                    },
                    "N3_BillingProviderAddress": {
                      "ResponseContactAddressLine_01": "1234 SOME ROAD"
                    },
                    "N4_BillingProviderCity_State_ZIPCode": {
                      "AdditionalPatientInformationContactCityName_01": "CHICAGO",
                      "AdditionalPatientInformationContactStateCode_02": "IL",
                      "AdditionalPatientInformationContactPostalZoneorZIPCode_03": "606739999"
                    },
                    "AllREF": {
                      "REF_BillingProviderTaxIdentification": {
                        "ReferenceIdentificationQualifier_01": "EI",
                        "MemberGrouporPolicyNumber_02": "999999999"
                      }
                    }
                  }
                },
                "Loop2000B": [
                  {
                    "HL_SubscriberHierarchicalLevel": {
                      "HierarchicalIDNumber_01": "2",
                      "HierarchicalParentIDNumber_02": "1",
                      "HierarchicalLevelCode_03": "22",
                      "HierarchicalChildCode_04": "0"
                    },
                    "SBR_SubscriberInformation": {
                      "PayerResponsibilitySequenceNumberCode_01": "P",
                      "IndividualRelationshipCode_02": "18",
                      "ClaimFilingIndicatorCode_09": "12"
                    },
                    "AllNM1": {
                      "Loop2010BA": {
                        "NM1_SubscriberName": {
                          "EntityIdentifierCode_01": "IL",
                          "EntityTypeQualifier_02": "1",
                          "ResponseContactLastorOrganizationName_03": "BLOGGS",
                          "ResponseContactFirstName_04": "JOE",
                          "IdentificationCodeQualifier_08": "MI",
                          "ResponseContactIdentifier_09": "1234567890"
                        },
                        "N3_SubscriberAddress": {
                          "ResponseContactAddressLine_01": "1 SOME BLVD"
                        },
                        "N4_SubscriberCity_State_ZIPCode": {
                          "AdditionalPatientInformationContactCityName_01": "CHICAGO",
                          "AdditionalPatientInformationContactStateCode_02": "IL",
                          "AdditionalPatientInformationContactPostalZoneorZIPCode_03": "606129998"
                        },
                        "DMG_SubscriberDemographicInformation": {
                          "DateTimePeriodFormatQualifier_01": "D8",
                          "DependentBirthDate_02": "19570111",
                          "DependentGenderCode_03": "M"
                        }
                      },
                      "Loop2010BB": {
                        "NM1_PayerName": {
                          "EntityIdentifierCode_01": "PR",
                          "EntityTypeQualifier_02": "2",
                          "ResponseContactLastorOrganizationName_03": "PAYER",
                          "IdentificationCodeQualifier_08": "PI",
                          "ResponseContactIdentifier_09": "12345"
                        },
                        "N3_PayerAddress": {
                          "ResponseContactAddressLine_01": "1 PAYER WAY"
                        },
                        "N4_PayerCity_State_ZIPCode": {
                          "AdditionalPatientInformationContactCityName_01": "ST LOUIS",
                          "AdditionalPatientInformationContactStateCode_02": "MO",
                          "AdditionalPatientInformationContactPostalZoneorZIPCode_03": "212441850"
                        },
                        "AllREF": {
                          "REF_PayerSecondaryIdentification": [
                            {
                              "ReferenceIdentificationQualifier_01": "2U",
                              "MemberGrouporPolicyNumber_02": "W1014"
                            }
                          ]
                        }
                      }
                    },
                    "Loop2300": [
                      {
                        "CLM_ClaimInformation": {
                          "PatientControlNumber_01": "1000A",
                          "TotalClaimChargeAmount_02": "140",
                          "HealthCareServiceLocationInformation_05": {
                            "FacilityTypeCode_01": "19",
                            "FacilityCodeQualifier_02": "B",
                            "ClaimFrequencyTypeCode_03": "1"
                          },
                          "ProviderorSupplierSignatureIndicator_06": "Y",
                          "AssignmentorPlanParticipationCode_07": "A",
                          "BenefitsAssignmentCertificationIndicator_08": "Y",
                          "ReleaseofInformationCode_09": "Y"
                        },
                        "AllHI": {
                          "HI_HealthCareDiagnosisCode": {
                            "HealthCareCodeInformation_01": {
                              "CodeListQualifierCode_01": "ABK",
                              "IndustryCode_02": "I10"
                            }
                          }
                        },
                        "Loop2400": [
                          {
                            "LX_ServiceLineNumber": {
                              "AssignedNumber_01": "1"
                            },
                            "SV1_ProfessionalService": {
                              "CompositeMedicalProcedureIdentifier_01": {
                                "ProductorServiceIDQualifier_01": "HC",
                                "ProcedureCode_02": "99213"
                              },
                              "LineItemChargeAmount_02": "140",
                              "UnitorBasisforMeasurementCode_03": "UN",
                              "ServiceUnitCount_04": "1",
                              "CompositeDiagnosisCodePointer_07": {
                                "DiagnosisCodePointer_01": "1"
                              }
                            },
                            "AllDTP": {
                              "DTP_Date_ServiceDate": {
                                "DateTimeQualifier_01": "472",
                                "DateTimePeriodFormatQualifier_02": "D8",
                                "DateTimePeriod_03": "20151124"
                              }
                            }
                          }
                        ]
                      }
                    ]
                  },
                  {
                    "HL_SubscriberHierarchicalLevel": {
                      "HierarchicalIDNumber_01": "3",
                      "HierarchicalParentIDNumber_02": "1",
                      "HierarchicalLevelCode_03": "22",
                      "HierarchicalChildCode_04": "0"
                    },
                    "SBR_SubscriberInformation": {
                      "PayerResponsibilitySequenceNumberCode_01": "P",
                      "IndividualRelationshipCode_02": "18",
                      "ClaimFilingIndicatorCode_09": "12"
                    },
                    "AllNM1": {
                      "Loop2010BA": {
                        "NM1_SubscriberName": {
                          "EntityIdentifierCode_01": "IL",
                          "EntityTypeQualifier_02": "1",
                          "ResponseContactLastorOrganizationName_03": "BLOGGS",
                          "ResponseContactFirstName_04": "FRED",
                          "IdentificationCodeQualifier_08": "MI",
                          "ResponseContactIdentifier_09": "9876543201"
                        },
                        "N3_SubscriberAddress": {
                          "ResponseContactAddressLine_01": "1 ANOTHER STR"
                        },
                        "N4_SubscriberCity_State_ZIPCode": {
                          "AdditionalPatientInformationContactCityName_01": "CHICAGO",
                          "AdditionalPatientInformationContactStateCode_02": "IL",
                          "AdditionalPatientInformationContactPostalZoneorZIPCode_03": "606129998"
                        },
                        "DMG_SubscriberDemographicInformation": {
                          "DateTimePeriodFormatQualifier_01": "D8",
                          "DependentBirthDate_02": "19700601",
                          "DependentGenderCode_03": "M"
                        }
                      },
                      "Loop2010BB": {
                        "NM1_PayerName": {
                          "EntityIdentifierCode_01": "PR",
                          "EntityTypeQualifier_02": "2",
                          "ResponseContactLastorOrganizationName_03": "PAYER",
                          "IdentificationCodeQualifier_08": "PI",
                          "ResponseContactIdentifier_09": "12345"
                        },
                        "N3_PayerAddress": {
                          "ResponseContactAddressLine_01": "1 PAYER WAY"
                        },
                        "N4_PayerCity_State_ZIPCode": {
                          "AdditionalPatientInformationContactCityName_01": "ST LOUIS",
                          "AdditionalPatientInformationContactStateCode_02": "MO",
                          "AdditionalPatientInformationContactPostalZoneorZIPCode_03": "212441850"
                        },
                        "AllREF": {
                          "REF_PayerSecondaryIdentification": [
                            {
                              "ReferenceIdentificationQualifier_01": "2U",
                              "MemberGrouporPolicyNumber_02": "W1014"
                            }
                          ]
                        }
                      }
                    },
                    "Loop2300": [
                      {
                        "CLM_ClaimInformation": {
                          "PatientControlNumber_01": "1001A",
                          "TotalClaimChargeAmount_02": "140",
                          "HealthCareServiceLocationInformation_05": {
                            "FacilityTypeCode_01": "19",
                            "FacilityCodeQualifier_02": "B",
                            "ClaimFrequencyTypeCode_03": "1"
                          },
                          "ProviderorSupplierSignatureIndicator_06": "Y",
                          "AssignmentorPlanParticipationCode_07": "A",
                          "BenefitsAssignmentCertificationIndicator_08": "Y",
                          "ReleaseofInformationCode_09": "Y"
                        },
                        "AllHI": {
                          "HI_HealthCareDiagnosisCode": {
                            "HealthCareCodeInformation_01": {
                              "CodeListQualifierCode_01": "ABK",
                              "IndustryCode_02": "I10"
                            }
                          }
                        },
                        "Loop2400": [
                          {
                            "LX_ServiceLineNumber": {
                              "AssignedNumber_01": "1"
                            },
                            "SV1_ProfessionalService": {
                              "CompositeMedicalProcedureIdentifier_01": {
                                "ProductorServiceIDQualifier_01": "HC",
                                "ProcedureCode_02": "99213"
                              },
                              "LineItemChargeAmount_02": "140",
                              "UnitorBasisforMeasurementCode_03": "UN",
                              "ServiceUnitCount_04": "1",
                              "CompositeDiagnosisCodePointer_07": {
                                "DiagnosisCodePointer_01": "1"
                              }
                            },
                            "AllDTP": {
                              "DTP_Date_ServiceDate": {
                                "DateTimeQualifier_01": "472",
                                "DateTimePeriodFormatQualifier_02": "D8",
                                "DateTimePeriod_03": "20151124"
                              }
                            }
                          }
                        ]
                      }
                    ]
                  }
                ]
              }
            ],
            "SE": {
              "NumberofIncludedSegments_01": "41",
              "TransactionSetControlNumber_02": "1239"
            },
            "Model": "EdiNation.X12.HIPAA.005010"
          }
    """;
}

