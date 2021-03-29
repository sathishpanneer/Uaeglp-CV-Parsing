using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using System.Linq;
using Uaeglp.LangModel;
using System.Data;
using System.Reflection;

namespace Uaeglp.Services
{

    public class LangService : ILangService
    {
        private Repositories.LangAppDbContext _appContext;
        private AppSettings _settings;

        public LangService(Repositories.LangAppDbContext appContext, IOptions<AppSettings> settings)
        {
            _appContext = appContext;
            _settings = settings.Value;
        }

        public async Task<IResponse<List<ViewModels.Page>>> GetLabels(ViewModels.PageLabelReq view)
        {
            var PageLabelRes = new List<Page>();
            DataTable dt = new DataTable();
            if (view.FromDate != null)
            {
                var data = from file in _appContext.Languages_Label
                           where file.Is_Active == 1 && file.Updated_Date >= view.FromDate
                           orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                           select new
                           {
                               Page_Name = file.Pagename,
                               Page_Display_Name = file.Page_Displayname,
                               Label_Name = file.Labelname,
                               file.Value,
                               Language = file.Language_Code
                           };
                var labelInfo = await data.ToListAsync();
                dt = ToDataTable(labelInfo);
            }
            else if(view.page_name == "ALL")
            {
                var data = from file in _appContext.Languages_Label
                           where file.Is_Active == 1
                           orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                           select new
                           {
                               Page_Name = file.Pagename,
                               Page_Display_Name = file.Page_Displayname,
                               Label_Name = file.Labelname,
                               file.Value,
                               Language = file.Language_Code
                           };
                var labelInfo = await data.ToListAsync();

                dt = ToDataTable(labelInfo);
            }

            String previouspagename = String.Empty;
            String previouslabelname = String.Empty;
            String NextLabelName = String.Empty;


            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                Page Response = new Page();
                Response.Name = dt.Rows[i]["Page_Name"].ToString();
                Response.DispName = dt.Rows[i]["Page_Display_Name"].ToString();

                if (String.IsNullOrEmpty(previouspagename))
                {
                    previouspagename = dt.Rows[i]["Page_Name"].ToString();
                }

                List<Label> lstlabel = new List<Label>();
                int CCount = i;
                if(i != 0)
                {
                    i = i - 1;
                }
                if (previouspagename.ToUpper() == dt.Rows[i]["Page_Name"].ToString().ToUpper())
                {
                    for (int j = i; j < dt.Rows.Count - 1; j++)
                    {
                        if(previouspagename.ToUpper() == dt.Rows[j]["Page_Name"].ToString().ToUpper())
                        {
                            String CurrentLabelName = dt.Rows[j]["Label_Name"].ToString();

                            if (previouslabelname.ToUpper() != CurrentLabelName.ToUpper())
                            {
                                if (dt.Rows[j]["Language"].ToString().Trim() == "en")
                                {
                                    Label label = new Label();
                                    label.Text = CurrentLabelName;
                                    List<Values> lstval = new List<Values>();
                                    Values val = new Values();

                                    val.En = dt.Rows[j]["Value"].ToString();
                                    if (dt.Rows[j + 1]["Language"].ToString().Trim() == "ar" && CurrentLabelName == dt.Rows[j + 1]["Label_Name"].ToString())
                                    {
                                        val.Ar = dt.Rows[j + 1]["Value"].ToString();
                                    }
                                    else
                                    {
                                        val.Ar = "";
                                    }

                                    lstval.Add(val);
                                    label.Values = lstval;
                                    lstlabel.Add(label);
                                }
                                else if (dt.Rows[j]["Language"].ToString().Trim() == "ar")
                                {
                                    Label label = new Label();
                                    label.Text = CurrentLabelName;
                                    List<Values> lstval = new List<Values>();
                                    Values val = new Values();

                                    val.Ar = dt.Rows[j]["Value"].ToString();
                                    if (dt.Rows[j + 1]["Language"].ToString().Trim() == "en" && CurrentLabelName == dt.Rows[j + 1]["Label_Name"].ToString())
                                    {
                                        val.En = dt.Rows[j + 1]["Value"].ToString();
                                    }
                                    else
                                    {
                                        val.En = "";
                                    }

                                    lstval.Add(val);
                                    label.Values = lstval;
                                    lstlabel.Add(label);
                                }
                                previouslabelname = CurrentLabelName;
                            }
                        }
                        else {
                            CCount = j;
                            break; 
                        }
                        CCount = j;
                    }
                }
                i = CCount;

                previouspagename = dt.Rows[i]["Page_Name"].ToString();

                Response.Labels = lstlabel;

                PageLabelRes.Add(Response);

            }
            
            return new LangResponse(PageLabelRes);
        }

        public async Task<IResponse<ViewModels.PageNew>> GetLabel(ViewModels.PageLabelReq view)
        {
            var PageLabelRes = new PageNew();
            DataTable dt = new DataTable();
            Dictionary<string, string> PageRes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (view.FromDate != null)
            {
                var data = from file in _appContext.Languages_Label
                           where file.Is_Active == 1 && file.Updated_Date >= view.FromDate
                           orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                           select new
                           {
                               Key = file.Pagename.Trim() + "_" + file.Labelname.Trim() + "_" + file.Language_Code.Trim(),
                               value = file.Value.Trim()
                           };
                var labelInfo = await data.ToListAsync();
                PageLabelRes.PageKeyValue = new Dictionary<string, string>();
                dt = ToDataTable(labelInfo);
                foreach (DataRow dr in dt.AsEnumerable())
                {
                    if (!PageLabelRes.PageKeyValue.ContainsKey(dr["Key"].ToString().Trim()))
                    {
                        PageLabelRes.PageKeyValue.Add(dr["Key"].ToString().Trim(), dr["value"].ToString().Trim());
                    }
                }

            }
            else if (view.page_name == "ALL")
            {
                var data = from file in _appContext.Languages_Label
                           where file.Is_Active == 1
                           orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                           select new 
                           {
                               Key = file.Pagename.Trim() + "_" + file.Labelname.Trim() + "_" + file.Language_Code.Trim(),
                               value = file.Value.Trim()
                           };
                var labelInfo = await data.ToListAsync();
                PageLabelRes.PageKeyValue = new Dictionary<string, string>();
                dt = ToDataTable(labelInfo);
                foreach (DataRow dr in dt.AsEnumerable())
                {
                    if (!PageLabelRes.PageKeyValue.ContainsKey(dr["Key"].ToString().Trim()))
                    {
                        PageLabelRes.PageKeyValue.Add(dr["Key"].ToString().Trim(), dr["value"].ToString().Trim());
                    }
                }
            }
            else
            {
                var data = from file in _appContext.Languages_Label
                           where file.Is_Active == 1 & file.Pagename == view.page_name || file.Pagename == "Common"
                           orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                           select new
                           {
                               Key = file.Pagename.Trim()+"_"+file.Labelname.Trim()+"_"+file.Language_Code.Trim(),
                               value = file.Value.Trim()
                           };
                var labelInfo = await data.ToListAsync();
                dt = ToDataTable(labelInfo);
                PageLabelRes.PageKeyValue = new Dictionary<string, string>();
                foreach (DataRow dr in dt.AsEnumerable())
                {
                    if (!PageLabelRes.PageKeyValue.ContainsKey(dr["Key"].ToString().Trim()))
                    {
                        PageLabelRes.PageKeyValue.Add(dr["Key"].ToString().Trim(), dr["value"].ToString().Trim());
                    }
                }
            }

            return new LanggResponse(PageLabelRes);
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public async Task<IResponse<List<ViewModels.LabelNames>>> GetLabelsUI(ViewModels.PageLabelReq view)
        {
            var PageLabelRes = new List<LabelNames>();
            var PageLabelModel = new List<Languages_Label>();
            if (view.page_name == "ALL")
            {
                var EnData = (from file in _appContext.Languages_Label
                              where file.Is_Active == 1 & file.Language_Code == "En"
                              orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                              select new
                              {
                                  Page_Name = file.Pagename,
                                  Page_Display_Name = file.Page_Displayname,
                                  Label_Name = file.Labelname,
                                  Value = file.Value,
                                  LanguageCode = file.Language_Code
                              });
                var ArData = (from file in _appContext.Languages_Label
                              where file.Is_Active == 1 & file.Language_Code == "Ar"
                              orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                              select new
                              {
                                  Page_Name = file.Pagename,
                                  Page_Display_Name = file.Page_Displayname,
                                  Label_Name = file.Labelname,
                                  Value = file.Value,
                                  LanguageCode = file.Language_Code
                              });
                var FinalData = from item in EnData
                                join item1 in ArData
                                on new { item.Page_Name, item.Label_Name } equals new { item1.Page_Name, item1.Label_Name }
                                select new
                                {
                                    PageName = item.Page_Name,
                                    PageDisplayName = item.Page_Display_Name,
                                    LabelName = item.Label_Name,
                                    English = item.Value,
                                    Arabic = item1.Value
                                };
                foreach (var item in FinalData)
                {
                    LabelNames ll = new LabelNames();
                    ll.PageName = item.PageName;
                    ll.PageDisplayName = item.PageDisplayName;
                    ll.LabelName = item.LabelName;
                    ll.English = item.English.ToString().Trim();
                    ll.Arabic = item.Arabic.ToString().Trim();

                    PageLabelRes.Add(ll);
                }
            }
            else
            {
                var EnData = from file in _appContext.Languages_Label
                             where file.Is_Active == 1 & file.Language_Code == "En" & file.Pagename == view.page_name || file.Pagename == "Common"
                             orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                             select new
                             {
                                 Page_Name = file.Pagename,
                                 Page_Display_Name = file.Page_Displayname,
                                 Label_Name = file.Labelname,
                                 Value = file.Value,
                                 LanguageCode = file.Language_Code
                             };
                var ArData = from file in _appContext.Languages_Label
                             where file.Is_Active == 1 & file.Language_Code == "Ar" & file.Pagename == view.page_name || file.Pagename == "Common"
                             orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                             select new
                             {
                                 Page_Name = file.Pagename,
                                 Page_Display_Name = file.Page_Displayname,
                                 Label_Name = file.Labelname,
                                 Value = file.Value,
                                 LanguageCode = file.Language_Code
                             };
                var FinalData = from item in EnData
                                join item1 in ArData
                                on new { item.Label_Name } equals new { item1.Label_Name }
                                select new
                                {
                                    PageName = item.Page_Name,
                                    PageDisplayName = item.Page_Display_Name,
                                    LabelName = item.Label_Name,
                                    English = item.Value,
                                    Arabic = item1.Value
                                };
                foreach (var item in FinalData)
                {
                    LabelNames ll = new LabelNames();
                    ll.PageName = item.PageName;
                    ll.PageDisplayName = item.PageDisplayName;
                    ll.LabelName = item.LabelName;
                    ll.English = item.English.ToString().Trim();
                    ll.Arabic = item.Arabic.ToString().Trim();

                    PageLabelRes.Add(ll);
                }

            }

            return new LangResponseUI(PageLabelRes);
        }

        public async Task<IResponse<List<ViewModels.LabelNames>>> SaveLabel(ViewModels.LabelNames view)
        {
            var PageLabelRes = new List<LabelNames>();

            var data = _appContext.Languages_Label.Where(x => x.Pagename == view.PageName & x.Labelname == view.LabelName).ToList();
            if (!data.Any())
            {
                Languages_Label labelEn = new Languages_Label();
                Languages_Label labelAr = new Languages_Label();

                labelEn.Pagename = view.PageName;
                labelEn.Page_Displayname = view.PageDisplayName;
                labelEn.Labelname = view.LabelName;
                labelEn.Language_Code = "en";
                labelEn.Value = view.English;
                labelEn.Is_Active = 1;
                labelEn.Created_By = 1;
                labelEn.Created_Date = DateTime.Now;
                labelEn.Updated_By = 1;
                labelEn.Updated_Date = DateTime.Now;

                labelAr.Pagename = view.PageName;
                labelAr.Page_Displayname = view.PageDisplayName;
                labelAr.Labelname = view.LabelName;
                labelAr.Language_Code = "ar";
                labelAr.Value = view.Arabic;
                labelAr.Is_Active = 1;
                labelAr.Created_By = 1;
                labelAr.Created_Date = DateTime.Now;
                labelAr.Updated_By = 1;
                labelAr.Updated_Date = DateTime.Now;

                _appContext.Languages_Label.AddRange(labelEn);
                _appContext.Languages_Label.AddRange(labelAr);
                _appContext.SaveChanges();
            }
            else
            {
                try
                {
                    foreach (var item in data)
                    {
                        if (item.Language_Code.ToString().Trim() == "en")
                        {
                            item.Pagename = view.PageName;
                            item.Page_Displayname = view.PageDisplayName;
                            item.Labelname = view.LabelName;
                            item.Value = view.English;
                            item.Updated_Date = DateTime.Now;
                        }
                        else
                        {
                            item.Pagename = view.PageName;
                            item.Page_Displayname = view.PageDisplayName;
                            item.Labelname = view.LabelName;
                            item.Value = view.Arabic;
                            item.Updated_Date = DateTime.Now;
                        }
                        _appContext.SaveChanges();
                    }
                }
                catch (Exception e)
                {

                }
            }

            return new LangResponseUI(PageLabelRes);
        }

        public async Task<IResponse<List<ViewModels.PageNames>>> GetPageNamesUI()
        {
            var PageLabelRes = new List<PageNames>();
            var data = from file in _appContext.Languages_Label
                       where file.Is_Active == 1
                       orderby file.Pagename ascending, file.Labelname descending, file.Language_Code descending
                       select new
                       {
                           Page_Name = file.Pagename
                       };
            var distinctData = data.Distinct().ToList();
            foreach (var item in distinctData)
            {
                PageNames pn = new PageNames();
                pn.PageName = item.Page_Name;

                PageLabelRes.Add(pn);
            }
            return new PageResponseUI(PageLabelRes);
        }
    }
}
