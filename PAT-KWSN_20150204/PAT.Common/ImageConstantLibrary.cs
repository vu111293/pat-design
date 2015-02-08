using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PAT.Common
{
    public partial class ImageConstantLibrary : Component
    {
        public ImageConstantLibrary()
        {
            InitializeComponent();
        }

        public ImageConstantLibrary(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
