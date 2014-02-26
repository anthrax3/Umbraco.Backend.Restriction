using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Umbraco.Backend.Restriction.WebAppTest
{
    public partial class error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.StatusCode = 500;
            throw new ApplicationException("forced Exception, to check 5xx status.");
        }
    }
}