// Ignore Spelling: Curp Mx

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class MxCurpStateCodesTests
{
   #region GetAllStateCodes Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurpStateCodes_GetAllStateCodes_ShouldReturnFullListOfStateCodes()
   {
      // Arrange.
      String[] expectedStateCodes =
      [
         "AS", "BC", "BS", "CC", "CL", "CM", "CS", "CH", "DF", "DG",
         "GT", "GR", "HG", "JC", "MC", "MN", "MS", "NT", "NL", "OC",
         "PL", "QT", "QR", "SP", "SL", "SR", "TC", "TS", "TL", "VZ",
         "YN", "ZS", "NE"
      ];

      // Act.
      var stateCodes = MxCurpStateCodes.GetAllStateCodes();

      // Assert.
      stateCodes.Should().BeEquivalentTo(expectedStateCodes);
   }

   #endregion

   #region GetStateName Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("AS", "Aguascalientes")]
   [InlineData("BC", "Baja California")]
   [InlineData("BS", "Baja California Sur")]
   [InlineData("CC", "Campeche")]
   [InlineData("CL", "Coahuila de Zaragoza")]
   [InlineData("CM", "Colima")]
   [InlineData("CS", "Chiapas")]
   [InlineData("CH", "Chihuahua")]
   [InlineData("DF", "Ciudad de México")]
   [InlineData("DG", "Durango")]
   [InlineData("GT", "Guanajuato")]
   [InlineData("GR", "Guerrero")]
   [InlineData("HG", "Hidalgo")]
   [InlineData("JC", "Jalisco")]
   [InlineData("MC", "México")]
   [InlineData("MN", "Michoacán de Ocampo")]
   [InlineData("MS", "Morelos")]
   [InlineData("NT", "Nayarit")]
   [InlineData("NL", "Nuevo León")]
   [InlineData("OC", "Oaxaca")]
   [InlineData("PL", "Puebla")]
   [InlineData("QT", "Querétaro")]
   [InlineData("QR", "Quintana Roo")]
   [InlineData("SP", "San Luis Potosí")]
   [InlineData("SL", "Sinaloa")]
   [InlineData("SR", "Sonora")]
   [InlineData("TC", "Tabasco")]
   [InlineData("TS", "Tamaulipas")]
   [InlineData("TL", "Tlaxcala")]
   [InlineData("VZ", "Veracruz")]
   [InlineData("YN", "Yucatán")]
   [InlineData("ZS", "Zacatecas")]
   [InlineData("NE", "Nacido en el Extranjero")]
   public void MxCurpStateCodes_GetStateName_ShouldReturnStateName_WhenStateCodeIsValid(
      String stateCode,
      String expectedStateName)
   {
      // Arrange.
      ReadOnlySpan<Char> span = stateCode.AsSpan();

      // Act.
      var stateName = MxCurpStateCodes.GetStateName(span);

      // Assert.
      stateName.Should().Be(expectedStateName);
   }

   [Fact]
   public void MxCurpStateCodes_GetStateName_ShouldReturnStringEmpty_WhenStateCodeIsInvalid()
   {
      // Arrange.
      ReadOnlySpan<Char> span = "DC".AsSpan();

      // Act.
      var stateName = MxCurpStateCodes.GetStateName(span);

      // Assert.
      stateName.Should().BeEmpty();
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void MxCurpStateCodes_GetStateName_ShouldReturnStringEmpty_WhenStateCodeIsEmpty(String? stateCode)
   {
      // Arrange.
      ReadOnlySpan<Char> span = stateCode.AsSpan();

      // Act.
      var stateName = MxCurpStateCodes.GetStateName(span);

      // Assert.
      stateName.Should().BeEmpty();
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("AS")]
   [InlineData("BC")]
   [InlineData("BS")]
   [InlineData("CC")]
   [InlineData("CL")]
   [InlineData("CM")]
   [InlineData("CS")]
   [InlineData("CH")]
   [InlineData("DF")]
   [InlineData("DG")]
   [InlineData("GT")]
   [InlineData("GR")]
   [InlineData("HG")]
   [InlineData("JC")]
   [InlineData("MC")]
   [InlineData("MN")]
   [InlineData("MS")]
   [InlineData("NT")]
   [InlineData("NL")]
   [InlineData("OC")]
   [InlineData("PL")]
   [InlineData("QT")]
   [InlineData("QR")]
   [InlineData("SP")]
   [InlineData("SL")]
   [InlineData("SR")]
   [InlineData("TC")]
   [InlineData("TS")]
   [InlineData("TL")]
   [InlineData("VZ")]
   [InlineData("YN")]
   [InlineData("ZS")]
   [InlineData("NE")]
   public void MxCurpStateCodes_ValidateStateCode_ShouldReturnTrue_WhenStateCodeIsValid(String stateCode)
   {
      // Arrange.
      ReadOnlySpan<Char> span = stateCode.AsSpan();

      // Act/assert.
      MxCurpStateCodes.ValidateStateCode(span).Should().BeTrue();
   }

   [Fact]
   public void MxCurpStateCodes_ValidateStateCode_ShouldReturnFalse_WhenStateCodeIsInvalid()
   {
      // Arrange.
      ReadOnlySpan<Char> span = "DC".AsSpan();

      // Act/assert.
      MxCurpStateCodes.ValidateStateCode(span).Should().BeFalse();
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void MxCurpStateCodes_Validate_ShouldReturnFalse_WhenStateCodeIsEmpty(String? stateCode)
   {
      // Arrange.
      ReadOnlySpan<Char> span = stateCode.AsSpan();

      // Act/assert.
      MxCurpStateCodes.ValidateStateCode(span).Should().BeFalse();
   }

   #endregion
}
