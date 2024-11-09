using Xunit;
using Xunit.Abstractions;

namespace psn.PH;

public class EmailExtended_ExtTests
{
    private readonly ITestOutputHelper output;

    public EmailExtended_ExtTests(ITestOutputHelper output)
    {
        this.output = output;
    }

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

    [Fact]
    public void isValidateEmailAddress_Ext_test9()
    {
        var ee = new EmailExtended_Ext();
        string test_address = "nobody@cornerstone.team";
        Assert.False(ee.isValidateEmailAddress_Ext(test_address));
    }

    [Fact]
    /// <summary>
    /// simulate the case of sending email with server validation turned off but
    /// going on TLS. The smtp server will be on self signed certificate. 
    /// </summary>
    public void sendEmail_Ext_test1()
    {
        var ee = new EmailExtended_Ext();
        string[] to = new string[1] { "Joao@outsystems.com" };
        string[] cc = new string[] { };
        string[] bcc = new string[] { };
        string[] replyTo = new string[] { };
        string subject = "Test mail";
        bool isHtml = true;
        string content = "<html><body>Hello World 1!</body></html>";
        // ignore server certification validation since it is self signed cert
        // but going on TLS
        var status = ee.sendEmail_Ext("127.0.0.1", 587, "nobody", "badpassword", "nobody@nowhere.com", to, cc, bcc, replyTo, subject, isHtml, content, true, false);
        Assert.True(status);
    }

    [Fact]
    /// <summary>
    /// simulate the case of sending email with server validation turned on 
    /// however, for unit test, the smtp server is using self signed certificate 
    /// and hence will fail in server certificate validation, throwing a authenticationexception
    /// </summary>
    public void sendEmail_Ext_test2()
    {
        var ee = new EmailExtended_Ext();
        string[] to = new string[1] { "Joao@outsystems.com" };
        string[] cc = new string[] { };
        string[] bcc = new string[] { };
        string[] replyTo = new string[] { };
        string subject = "Test mail";
        bool isHtml = true;
        string content = "<html><body>Hello World 2!</body></html>";

        Assert.Throws<System.Security.Authentication.AuthenticationException>(() => ee.sendEmail_Ext("127.0.0.1", 587, "nobody", "badpassword", "nobody@nowhere.com", to, cc, bcc, replyTo, subject, isHtml, content, false, false));
    }

    [Fact]
    public void getBuildInfo_test1()
    {
        var ee = new EmailExtended_Ext();
        var buildInfo = ee.getBuildInfo_Ext();
        Assert.True(buildInfo.Length == 14);
        output.WriteLine(buildInfo);
    }
}