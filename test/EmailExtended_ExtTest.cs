using Xunit;
using netDumbster.smtp;
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

    [Fact]
    public void isValidateEmailAddress_Ext_test8()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "João@outsystems.com";
        Assert.True(ee.isValidateEmailAddress_Ext(test_address));
    }

    public void sendEmail_Ext_test1()
    {
        var server = SimpleSmtpServer.Start(1234);
        var ee = new EmailExtended_Ext();
        string[] to = new string[1] { "João@outsystems.com" };
        string[] cc = new string[] { };
        string[] bcc = new string[] { };
        string subject = "Test mail";
        bool isHtml = true;
        string content = "<html><body>Hello World!</body></html>";
        var status = ee.sendEmail_Ext("127.0.0.1", 1234, "nobody", "badpassword", "nobody@nowhere.com", to, cc, bcc, subject, isHtml, content);
        var count = server.ReceivedEmailCount;
        Assert.False(count == 0);
    }

    [Fact]
    public void getBuildInfo_test1()
    {
        var ee = new EmailExtended_Ext();
        var buildInfo = ee.getBuildInfo_Ext();
        Assert.True(buildInfo.Length == 14);
    }
}