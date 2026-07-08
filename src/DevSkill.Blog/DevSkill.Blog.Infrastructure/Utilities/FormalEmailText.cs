using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Utilities
{
    public class FormalEmailText
    {
        public string Subject { get; }
        public string Body { get; }

        public FormalEmailText(string receiverName, string subject, string messageBody)
        {
            Subject = subject;

            Body = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f6f9;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }}
        .email-header {{
            background-color: #0d6efd;
            color: #ffffff;
            padding: 20px;
            text-align: center;
            font-size: 20px;
            font-weight: 600;
        }}
        .email-body {{
            padding: 30px;
            color: #333333;
            font-size: 15px;
            line-height: 1.6;
        }}
        .email-footer {{
            background-color: #f1f3f5;
            padding: 15px;
            text-align: center;
            font-size: 13px;
            color: #6c757d;
        }}
    </style>
</head>
<body>

    <div class='email-container'>
        <div class='email-header'>
            ThoughtShare
        </div>

        <div class='email-body'>
            <p>Hi <strong>{receiverName}</strong>,</p>

            <p>
                Thank you for reaching out to us.
            </p>

            <p>
                Below is our response:
            </p>

            <div style='background:#f8f9fa;padding:15px;border-radius:5px;'>
                {messageBody}
            </div>

            <p>
                If you have any further questions, feel free to reply.
            </p>

            <p>Best regards,<br/>
            <strong>ThoughtShare Team</strong></p>
        </div>

        <div class='email-footer'>
            © {DateTime.Now.Year} ThoughtShare. All rights reserved.
        </div>
    </div>

</body>
</html>";
        }
    }
}
