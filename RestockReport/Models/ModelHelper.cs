using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Models
{
    public class ModelHelper
    {
        //Fill Terminal Dropdownlist
        public List<SelectListItem> ToSelectTerminal(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.TerminalKey + " - " + v.LocationName, Value = v.TerminalKey });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> RelationshipOwner(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Companyname, Value = Convert.ToString(v.CompanyId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ProgramType(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Type, Value = Convert.ToString(v.ProgramId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }
        public List<SelectListItem> CommunicationType(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                    tempList.Add(new SelectListItem { Text = v.Type, Value = Convert.ToString(v.CommunicationId) });
                }
                tempList.TrimExcess();
            }
            return tempList;
        }


        public List<SelectListItem> ToSelectLocationTypeList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {


                    if (!tempList.Any(s => s.Value == v.LocationType))
                    {
                        tempList.Add(new SelectListItem { Text = v.LocationType, Value = Convert.ToString(v.LocationType) });
                    }

                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectRegionList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {

                    if (!tempList.Any(s => s.Value == v.Region))
                    {
                        tempList.Add(new SelectListItem { Text = v.Region, Value = Convert.ToString(v.Region) });
                    }


                    
                }
                tempList.TrimExcess();
            }
            return tempList;
        }





        public List<SelectListItem> ToSelectUserRegionList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {
                  
                        tempList.Add(new SelectListItem { Text = v.RegionName, Value = Convert.ToString(v.RegionId)});
                  



                }
                tempList.TrimExcess();
            }
            return tempList;
        }

        public List<SelectListItem> ToSelectMakeAndModelList(dynamic values)
        {
            List<SelectListItem> tempList = null;
            if (values != null)
            {
                tempList = new List<SelectListItem>();
                foreach (var v in values)
                {


                    if (!tempList.Any(s => s.Value == v.MakeAndModel))
                    {
                        tempList.Add(new SelectListItem { Text = v.MakeAndModel, Value = Convert.ToString(v.MakeAndModel) });
                    }
                    
                }
                tempList.TrimExcess();
            }
            return tempList;
        }
    }
}