﻿using System;

namespace WangJun.NC.YunDocument
{
    public static class Config
    {
        public static string DBConnection
        {
            get
            {
                return "Data Source=.\\SQL2017;Initial Catalog=WangJunDocument;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            }
        }
    }
}
