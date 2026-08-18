using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Enums;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <inheritdoc/>
    public class ContractEventMapper : IContractEventMapper
    {
        /// <summary>
        /// Organization Name Abbreviated.
        /// </summary>
        private const string OrganizationNameAbbreviated = "ESFA";

        /// <summary>
        /// CreatedBy.
        /// </summary>
        private const string CreatedBy = "Feed-ContractEventProcessor";

        /// <inheritdoc/>
        public CreateRequest GetCreateRequest(ContractEvent contractEvent)
        {
            try
            {
                string contractTitle = CreateContractTitle(contractEvent);

                var createRequest = new CreateRequest();
                createRequest.AmendmentType = (Data.Api.Client.Enumerations.ContractAmendmentType)contractEvent.AmendmentType;
                createRequest.ContractFundingStreamPeriodCodes = GetContractFundingStreamPeriodCodes(contractEvent.ContractAllocations);
                createRequest.ContractNumber = contractEvent.ContractNumber;
                createRequest.Year = FormatPeriod(contractEvent);
                createRequest.Type = GetContractType(contractEvent);
                createRequest.ContractVersion = contractEvent.ContractVersion;
                createRequest.EndDate = contractEvent.EndDate;
                createRequest.FundingType = (Data.Api.Client.Enumerations.ContractFundingType)contractEvent.FundingType;
                createRequest.ParentContractNumber = contractEvent.ParentContractNumber;
                createRequest.StartDate = contractEvent.StartDate;
                createRequest.Ukprn = contractEvent.Ukprn;
                createRequest.Value = contractEvent.Value;
                createRequest.ContractData = contractEvent.ContractEventXml;
                createRequest.Title = contractTitle;
                createRequest.ContractAllocationNumber = contractEvent.ContractAllocations?.FirstOrDefault()?.ContractAllocationNumber;
                createRequest.CreatedBy = CreatedBy;
                createRequest.PageCount = 0;
                createRequest.SignedOn = contractEvent.SignedOn;
                return createRequest;
            }
            catch (ContractEventExpectationFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ContractEventExpectationFailedException(contractEvent.BookmarkId, contractEvent.ContractNumber, contractEvent.ContractVersion, $"Unhandled exception trying to parse contract event message, see inner exception for details.", ex);
            }
        }

        /// <inheritdoc/>
        public string CreateContractTitle(ContractEvent contractEvent)
        {
            var fundingType = contractEvent.FundingType;

            var fundingTypeName = fundingType.GetEnumDisplayName();

            var lepArea = string.Empty;
            var specification = string.Empty;

            var contractAllocation = contractEvent.ContractAllocations?.FirstOrDefault();
            if (fundingType == ContractFundingType.Esf && contractAllocation != null)
            {
                lepArea = contractAllocation.LEPArea;
                specification = contractAllocation.TenderSpecTitle;
            }

            var variation = string.Empty;
            if (contractEvent.ContractVersion > 1)
            {
                variation = "variation ";
            }

            var contractPeriod = FormatPeriod(contractEvent);

            switch (fundingType)
            {
                case ContractFundingType.Mainstream:
                    if (GetStartingYear(contractEvent.ContractPeriodValue) >= 17)
                    {
                        return
                            $"Skills and adult education contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";
                    }

                    // All mainstream contracts are effectively variations so they always display variation in the friendly name.
                    return $"{fundingTypeName} contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.Age:
                    return $"{fundingTypeName} Grant contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.TwentyFourPlusLoan:
                    return $"Advanced Learner Loans contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.Eop:
                case ContractFundingType.Eof:
                    return $"{GetFriendlyYear(contractEvent.ContractPeriodValue)} {fundingTypeName} contract variation version {contractEvent.ContractVersion}";

                case ContractFundingType.Esf:
                    return $"{fundingTypeName} {specification} contract {variation}for {lepArea} version {contractEvent.ContractVersion}";

                case ContractFundingType.Levy:
                    return $"Apprenticeship agreement {DateTime.Now.ToFullMonthAndFullYearDisplay()} version {contractEvent.ContractVersion}";

                case ContractFundingType.Ncs:
                    return $"{fundingTypeName} contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.NonLevy:
                    return $"{OrganizationNameAbbreviated} apprenticeship contract for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.SixteenNineteenFunding:
                case ContractFundingType.Aebp:
                case ContractFundingType.ProcuredNineteenToTwentyFourTraineeship:
                case ContractFundingType.StrategicDevelopmentFund2:
                case ContractFundingType.FEReclassificationCapitalAllocation:
                case ContractFundingType.FECapitalTransformationFundAllocation:
                case ContractFundingType.AdultEducationBudgetProcured2023:
                case ContractFundingType.AdultSkillsFund:
                case ContractFundingType.AdultSkillsFundProcured2023:
                case ContractFundingType.DWPAdultSkillsFund:
                    return $"{fundingTypeName} contract for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.Nla:
                    return $"{fundingTypeName} version {contractEvent.ContractVersion}";

                case ContractFundingType.AdvancedLearnerLoans:
                    return $"Advanced Learner Loans contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.EducationAndSkillsFunding:
                case ContractFundingType.NonLearningGrant:
                case ContractFundingType.SixteenEighteenForensicUnit:
                case ContractFundingType.DanceAndDramaAwards:
                case ContractFundingType.CollegeCollaborationFund:
                case ContractFundingType.FurtherEducationConditionAllocation:
                    return $"{fundingTypeName} {variation}for {contractPeriod} version {contractEvent.ContractVersion}";

                case ContractFundingType.AdultEducationBudgetContractForService:
                case ContractFundingType.HigherTechnicalEducation:
                case ContractFundingType.SkillsAcceleratorDevelopment:
                case ContractFundingType.FurtherEducationProfessionalDevelopmentGrants:
                case ContractFundingType.HigherTechnicalEducationSkillsInjectionFund2:
                case ContractFundingType.TakingTeachingFurther:
                case ContractFundingType.TakingTeachingFurtherYear2:
                case ContractFundingType.ConstructionSkillsPackageIndustryPlacementsFund:
                    return $"{fundingTypeName} contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";
                case ContractFundingType.SkillsBootcamps:
                case ContractFundingType.SkillsBootcampsDPS:
                case ContractFundingType.AdditionalCapitalAllocations:
                case ContractFundingType.HigherTechnicalEducationSkillsInjectionFund:
                    return $"{fundingTypeName} contract {variation}version {contractEvent.ContractVersion}";
                case ContractFundingType.Multiply:
                    return $"{fundingTypeName} Programme contract {variation}for {contractPeriod} version {contractEvent.ContractVersion}";
                case ContractFundingType.ConstructionTechnicalExcellenceColleges:
                    return $"{fundingTypeName} - revenue contract for {contractPeriod} version {contractEvent.ContractVersion}";
                default:
                    throw new NotImplementedException($"Title for {nameof(fundingType)} with value {fundingType} is not implemented.");
            }
        }

        /// <inheritdoc/>
        public string FormatPeriod(ContractEvent contractEvent)
        {
            string periodValue = contractEvent.ContractPeriodValue;
            DateTime? startDate = contractEvent.StartDate;
            DateTime? endDate = contractEvent.EndDate;

            if (periodValue.Length != 4)
            {
                throw new ContractEventExpectationFailedException(contractEvent.BookmarkId, contractEvent.ContractNumber, contractEvent.ContractVersion, $"{nameof(contractEvent.ContractPeriodValue)} has invalid value [{contractEvent.ContractPeriodValue}]");
            }

            var startsWith20 = periodValue.Substring(0, 2) == "20";

            if (startsWith20)
            {
                var startYear = startDate.HasValue ? startDate.Value.Year : (int?)null;
                var endYear = endDate.HasValue ? endDate.Value.Year : (int?)null;

                if (startYear == endYear)
                {
                    return periodValue;
                }
            }

            return "20" + periodValue.Substring(0, 2) + " to 20" + periodValue.Substring(2, 2);
        }

        /// <inheritdoc/>
        public CreateContractCode[] GetContractFundingStreamPeriodCodes(IEnumerable<ContractAllocation> contractAllocations)
        {
            if (contractAllocations != null &&
                contractAllocations.Any(p => p.FundingStreamPeriodCode != null))
            {
                return contractAllocations
                        .Select(a => a.FundingStreamPeriodCode)
                        .Distinct()
                        .Select(c => new CreateContractCode { Code = c })
                        .ToArray();
            }

            return Array.Empty<CreateContractCode>();
        }

        /// <inheritdoc/>
        public int GetStartingYear(string periodValue)
        {
            if (periodValue.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(periodValue));
            }

            return int.Parse(periodValue.Substring(0, 2));
        }

        /// <inheritdoc/>
        public string GetFriendlyYear(string year)
        {
            if (year.Length < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            if (year.Length == 4)
            {
                return "20" + year.Substring(0, 2) + "/" + year.Substring(2, 2);
            }

            if (year.Substring(0, 2) == "20")
            {
                return year;
            }

            throw new ArgumentOutOfRangeException(nameof(year));
        }

        /// <inheritdoc/>
        public CreateContractRequestDocument GetContractContent(byte[] pdfADoc, string fileName)
        {
            var contractContent = new CreateContractRequestDocument();
            contractContent.FileName = fileName;
            contractContent.Size = pdfADoc.Length;
            contractContent.Content = pdfADoc;
            return contractContent;
        }

        /// <inheritdoc/>
        public string GetFileNameForContractDocument(int? ukprn, string contractNumber, int version)
        {
            return $"{ukprn}_{contractNumber}_v{version}.pdf";
        }

        /// <inheritdoc/>
        public string GetFolderNameForContractDocument(string fundingTypeShortName, string period, string suffix)
        {
            return $"{fundingTypeShortName}-{period}-{suffix}";
        }

        private static Data.Api.Client.Enumerations.ContractType GetContractType(ContractEvent contractEvent)
        {
            var contractTypes = Enum.GetValues(typeof(Enums.ContractType)).Cast<Enums.ContractType>();

            //SigleOrDefault is a workaround for known issue with ContractType enumeration used by MyESF and is added here for backward compatibility only, many feed types are not supported.
            //This field is not used in MyESF and should be considered for decommissioning in the future contracts database design.
            return (Data.Api.Client.Enumerations.ContractType)contractTypes.SingleOrDefault(e => e.GetEnumDisplayName().Equals(contractEvent.Type, StringComparison.OrdinalIgnoreCase));
        }
    }
}