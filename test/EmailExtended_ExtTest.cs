using Xunit;
namespace psn.PH;

public class EmailExtended_ExtTests
{
    /// <summary>
    /// Unit tests that are mostly inspired from https://www.w3resource.com/javascript/form/email-validation.php
    /// </summary>
    [Fact]
    public void isValidateEmailAddress_Ext_test1()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "mysite.ourearth.com";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }

    [Fact]
    public void isValidateEmailAddress_Ext_test2()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "mysite@.com.my";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }

    [Fact]
    public void isValidateEmailAddress_Ext_test3_1()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "mysite()*@gmail.com";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }


    [Fact]
    public void isValidateEmailAddress_Ext_test3_2()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "!wrong@company.com";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }

    [Fact]
    public void isValidateEmailAddress_Ext_test4()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "mysite123@gmail.b";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }


    [Fact]
    public void isValidateEmailAddress_Ext_test5()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "mysite@.org.org";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }

    [Fact]
    public void isValidateEmailAddress_Ext_test6()
    {
        var ee = new EmailExtended_Ext();
        string test_address = ".mysite@mysite.org";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }

    [Fact]
    public void isValidateEmailAddress_Ext_test7()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "mysite..1234@yahoo.com";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }
}