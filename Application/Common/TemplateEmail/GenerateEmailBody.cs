using System.Text;
using Application.DTOs.OrdersDto;

namespace Application.Common.TemplateEmail;

public static class GenerateEmailBody
{
    public static string GetEmailOrderStatusBody(string userName, Guid orderId,List<OrderDetailDto> items, decimal totalAmount)
    {
        // Load the template from the file
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OrderShippedTemplate.html");
        string body = File.ReadAllText(templatePath);

        // Replace placeholders with actual values
        body = body.Replace("{{UserName}}", userName);
        body = body.Replace("{{OrderId}}", orderId.ToString());
        // Generate the items list
        var itemList = new StringBuilder();
        foreach (var item in items)
        {
            itemList.AppendLine($"<li>{item.ProductName} - {item.Quantity} - {item.UnitPrice:C}</li>");
        }
        body = body.Replace("[Item List]", itemList.ToString());
        body = body.Replace("[Total Amount]", totalAmount.ToString("C"));
        return body;
    }
    public static string GetEmailOTPBody(string userName,string OTP)
    {
        // Load the template from the file
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OTPTemplate.html");
        string body = File.ReadAllText(templatePath);
        body = body.Replace("{{UserName}}", userName);
        body = body.Replace("{{OTP}}", OTP);
       
        return body;
    }
}