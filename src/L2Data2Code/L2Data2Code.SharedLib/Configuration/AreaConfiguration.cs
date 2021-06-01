using System;
using System.Collections.Generic;
using System.Text;

namespace L2Data2Code.SharedLib.Configuration
{
    public class AreaConfiguration
    {
        private string _outputDataSource;
        private string _descriptionDataSource;

        public string Name { get; set; }
        public string DataSource { get; set; }
        public string OutputDataSource { get => _outputDataSource ?? DataSource; set => _outputDataSource = value; }
        public string DescriptionsDataSource { get => _descriptionDataSource ?? DataSource; set => _descriptionDataSource = value; }

        public AreaConfiguration()
        {
        }

        public AreaConfiguration(string dataSource)
        {
            DataSource = dataSource;
        }
    }
}
