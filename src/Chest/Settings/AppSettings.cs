// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.


namespace Chest.Settings
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        
        public CqrsSettings CqrsSettings { get; set; }
    }
}
