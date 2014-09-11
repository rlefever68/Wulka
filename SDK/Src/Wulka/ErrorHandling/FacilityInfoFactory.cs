namespace Wulka.ErrorHandling
{
    /// <summary>
    /// Creates facility infos per given facility code.
    /// </summary>
    public static class FacilityInfoFactory
    {
        /// <summary>
        /// Creates facility infos per given facility code.
        /// </summary>
        /// <param name="facilityCode">The facility code.</param>
        /// <returns></returns>
        public static FacilityInfo CreateFacilityInfo(FacilityCode facilityCode)
        {
            return new FacilityInfo(facilityCode, "Wulka." + facilityCode);
        }
    }
}
