using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pds.Contracts.ContractEventProcessor.Services.Enums
{
    /// <summary>
    /// The types of funding that the system supports.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContractFundingType
    {
        /// <summary>
        /// Unknown funding type.
        /// </summary>
        [Display(Name = "", Description = "Unknown")]
        Unknown = 0,

        /// <summary>
        /// Mainstream funding type.
        /// </summary>
        [Display(Name = "Mainstream", Description = "Mainstream", ShortName = "MAIN")]
        Mainstream = 1,

        /// <summary>
        /// Esf funding type
        /// </summary>
        [Display(Name = "ESF", Description = "European social fund (ESF)", ShortName = "ESF")]
        Esf = 2,

        /// <summary>
        /// TwentyFourPlusLoan funding type
        /// </summary>
        [Display(Name = "24+LOANS", Description = "24+LOANS", ShortName = "24+LOANS")]
        TwentyFourPlusLoan = 3,

        /// <summary>
        /// Age funding type
        /// </summary>
        [Display(Name = "AGE", Description = "AGE Grant", ShortName = "AGE")]
        Age = 4,

        /// <summary>
        /// Eop funding type
        /// </summary>
        [Display(Name = "EOP", Description = "EOP", ShortName = "EOP")]
        Eop = 5,

        /// <summary>
        /// Eof funding type
        /// </summary>
        [Display(Name = "EOF", Description = "EOF", ShortName = "EOF")]
        Eof = 6,

        /// <summary>
        /// CityDeals funding type.
        /// </summary>
        [Display(Name = "City Deal", Description = "City deal", ShortName = "CityDeals")]
        CityDeals = 7,

        /// <summary>
        /// LocalGrowth funding type.
        /// </summary>
        [Display(Name = "Local Growth", Description = "Local growth", ShortName = "Localgrowth")]
        LocalGrowth = 8,

        /// <summary>
        /// Levy funding type.
        /// </summary>
        [Display(Name = "apprenticeship agreement", Description = "Apprenticeship agreement", ShortName = "LEVY")]
        Levy = 9,

        /// <summary>
        /// NCS funding type.
        /// </summary>
        [Display(Name = "National Careers Service (NCS)", Description = "National Careers Service (NCS)", ShortName = "NCS")]
        Ncs = 10,

        /// <summary>
        /// Non Levy funding type.
        /// </summary>
        [Display(Name = "Apprenticeship contract", Description = "Apprenticeship contract", ShortName = "NONLEVY")]
        NonLevy = 11,

        /// <summary>
        /// 1619Fund funding type.
        /// </summary>
        [Display(Name = "16 to 19 funding", Description = "16-19 funding", ShortName = "1619FUND")]
        SixteenNineteenFunding = 12,

        /// <summary>
        /// AEBP funding type.
        /// </summary>
        [Display(Name = "Procured adult education", Description = "Procured adult education)", ShortName = "AEB")]
        Aebp = 13,

        /// <summary>
        /// NLA funding type.
        /// </summary>
        [Display(Name = "Procured non levy apprenticeship contract", Description = "Procured non levy apprenticeship contract", ShortName = "NLA")]
        Nla = 14,

        /// <summary>
        /// Advanced Leaner Loans funding type
        /// </summary>
        [Display(Name = "Advanced Leaner Loans", Description = "Advanced Leaner Loans", ShortName = "LOANS")]
        AdvancedLearnerLoans = 15,

        /// <summary>
        /// Education and skills funding contract.
        /// </summary>
        [Display(Name = "Education and skills funding contract", Description = "Education and skills funding contract", ShortName = "EDSK")]
        EducationAndSkillsFunding = 16,

        /// <summary>
        /// Non-learning grant funding type.
        /// </summary>
        [Display(Name = "Non-learning grant funding agreement", Description = "Non-learning grant funding agreement", ShortName = "NLG")]
        NonLearningGrant = 17,

        /// <summary>
        /// 16-18 Forensic Unit.
        /// </summary>
        [Display(Name = "16 to 18 Forensic Unit", Description = "16-18 Forensic Unit", ShortName = "16-18FU")]
        SixteenEighteenForensicUnit = 18,

        /// <summary>
        /// Dance and Drama Awards.
        /// </summary>
        [Display(Name = "Dance and Drama Awards", Description = "Dance and Drama Awards", ShortName = "DADA")]
        DanceAndDramaAwards = 19,

        /// <summary>
        /// College Collaboration Fund Agreements.
        /// </summary>
        [Display(Name = "College Collaboration Fund", Description = "College Collaboration Fund", ShortName = "CCF")]
        CollegeCollaborationFund = 20,

        /// <summary>
        /// Further Education Condition Allocation agreements.
        /// </summary>
        [Display(Name = "Further Education Condition Allocation", Description = "Further Education Condition Allocation", ShortName = "FECA")]
        FurtherEducationConditionAllocation = 21,

        /// <summary>
        /// The procured nineteen to twenty four traineeship
        /// </summary>
        [Display(Name = "Procured 19 to 24 traineeship", Description = "Procured 19 to 24 traineeship", ShortName = "19TRN2020")]
        ProcuredNineteenToTwentyFourTraineeship = 22,

        /// <summary>
        /// Adult Education Budget (Contract for Service)
        /// </summary>
        [Display(Name = "ESFA Adult Education Budget (procured from Aug 2021)", Description = "ESFA Adult Education Budget (procured from Aug 2021)", ShortName = "AEB2021")]
        AdultEducationBudgetContractForService = 23,

        /// <summary>
        /// Higher technical education provider growth fund contract
        /// </summary>
        [Display(Name = "Higher technical education provider growth fund", Description = "Higher technical education provider growth fund", ShortName = "HTE-PGF")]
        HigherTechnicalEducation = 24,

        /// <summary>
        /// Skills accelerator development fund contract
        /// </summary>
        [Display(Name = "Skills accelerator development fund", Description = "Skills accelerator development fund", ShortName = "SADF")]
        SkillsAcceleratorDevelopment = 25,

        /// <summary>
        /// Further education professional development grants pilot contract
        /// </summary>
        [Display(Name = "Further education professional development grants pilot", Description = "Further education professional development grants", ShortName = "FE-PDGP")]
        FurtherEducationProfessionalDevelopmentGrants = 26,

        /// <summary>
        /// Further education professional development grants pilot contract
        /// </summary>
        [Display(Name = "Strategic Development Fund II", Description = "Strategic Development Fund II", ShortName = "SDFII")]
        StrategicDevelopmentFund2 = 27,

        /// <summary>
        /// Skills bootcamps contract
        /// </summary>
        [Display(Name = "Skills bootcamps", Description = "Skills bootcamps", ShortName = "SB")]
        SkillsBootcamps = 28,

        /// <summary>
        /// Multiply contract
        /// </summary>
        [Display(Name = "Multiply", Description = "Multiply", ShortName = "MULT")]
        Multiply = 29,

        /// <summary>
        /// Additional capital allocations.
        /// </summary>
        [Display(Name = "Additional capital allocations", Description = "Additional capital allocations", ShortName = "FE-ACA")]
        AdditionalCapitalAllocations = 30,

        /// <summary>
        /// Higher technical education skills injection fund.
        /// </summary>
        [Display(Name = "Higher technical education skills injection fund", Description = "Higher technical education skills injection fund", ShortName = "HTE-SIF")]
        HigherTechnicalEducationSkillsInjectionFund = 31,

        /// <summary>
        /// FE Reclassification Capital Allocation.
        /// </summary>
        [Display(Name = "FE Reclassification Capital Allocation", Description = "FE Reclassification Capital Allocation", ShortName = "FE-RCA")]
        FEReclassificationCapitalAllocation = 32,

        /// <summary>
        /// FE Capital Transformation Fund Allocation.
        /// </summary>
        [Display(Name = "FE Capital Transformation Fund Allocation", Description = "FE Capital Transformation Fund Allocation", ShortName = "FE-CTF")]
        FECapitalTransformationFundAllocation = 33,

        /// <summary>
        /// ESFA Adult Education Budget (procured from Aug 2023).
        /// Shares AEB2023 funding type from FCS with ESFA Adult Skills Fund (procured from Aug 2023).
        /// </summary>
        [Display(Name = "ESFA Adult Education Budget (procured from Aug 2023)", Description = "ESFA Adult Education Budget (procured from Aug 2023)", ShortName = "AEB2023")]
        AdultEducationBudgetProcured2023 = 34,

        /// <summary>
        /// Skills Bootcamps Dynamic Purchasing System.
        /// </summary>
        [Display(Name = "Skills bootcamps", Description = "Skills bootcamps dynamic purchasing system (DPS)", ShortName = "SBD")]
        SkillsBootcampsDPS = 35,

        /// <summary>
        /// Higher technical education skills injection fund 2.
        /// </summary>
        [Display(Name = "Higher technical education skills injection fund 2", Description = "Higher technical education skills injection fund 2", ShortName = "HTE-SIF2")]
        HigherTechnicalEducationSkillsInjectionFund2 = 36,

        /// <summary>
        /// ESFA Adult Skills Fund (procured from Aug 2023).
        /// Shares AEB2023 funding type from FCS with ESFA Adult Education Budget (procured from Aug 2023).
        /// </summary>
        [Display(Name = "ESFA Adult Skills Fund (procured from Aug 2023)", Description = "ESFA Adult Skills Fund (procured from Aug 2023)", ShortName = "AEB2023")]
        AdultSkillsFundProcured2023 = 37,

        /// <summary>
        /// Taking teaching further.
        /// </summary>
        [Display(Name = "Taking teaching further", Description = "Taking teaching further", ShortName = "TTF")]
        TakingTeachingFurther = 38,

        /// <summary>
        /// DfE Adult Skills Fund.
        /// Shares AEB2023 funding type from FCS with ESFA Adult Education Budget (procured from Aug 2023).
        /// </summary>
        [Display(Name = "DfE Adult Skills Fund", Description = "DfE Adult Skills Fund", ShortName = "AEB2023")]
        AdultSkillsFund = 39,

        /// <summary>
        /// Taking teaching further year 2.
        /// </summary>
        [Display(Name = "Taking teaching further", Description = "Taking teaching further", ShortName = "TTFY2")]
        TakingTeachingFurtherYear2 = 40,

        /// <summary>
        /// Construction Technical Excellence Colleges.
        /// </summary>
        [Display(Name = "Construction Technical Excellence Colleges", Description = "Construction Technical Excellence Colleges", ShortName = "CTEC")]
        ConstructionTechnicalExcellenceColleges = 41,

        /// <summary>
        /// Construction Skills Package: Industry Placements Fund.
        /// </summary>
        [Display(Name = "Construction Skills Package: Industry Placements Fund", Description = "Construction Skills Package: Industry Placements Fund", ShortName = "CSP-IPF")]
        ConstructionSkillsPackageIndustryPlacementsFund = 42,

        /// <summary>
        /// DWP Adult Skills Fund.
        /// Shares AEB2023 funding type from FCS with ESFA Adult Education Budget (procured from Aug 2023).
        /// </summary>
        [Display(Name = "DWP Adult Skills Fund", Description = "DWP Adult Skills Fund", ShortName = "AEB2023")]
        DWPAdultSkillsFund = 43
    }
}