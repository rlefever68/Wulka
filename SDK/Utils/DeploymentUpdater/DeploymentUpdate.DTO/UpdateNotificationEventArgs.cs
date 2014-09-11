using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Xml;
using System.Configuration;

namespace DeploymentUpdate.DTO
{
    public class UpdateNotificationEventArgs : EventArgs
    {
        public UpdateNotificationEventArgs(String updatePath, String message)
        {
            UpdatePath = updatePath;
            Message = message;
        }

        public String UpdatePath { get; set; }
        public String Message { get; set; }
    }
}
