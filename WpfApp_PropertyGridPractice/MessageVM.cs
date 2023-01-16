using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Trafix.CLRCommon;
using Trafix.OMSCommon;
using CLRRuleEngine=Trafix.CLRRuleEngine;
using Trafix.ReportClient.Model;
using WpfApp_PropertyGridPractice.NewFolder1;
using System.Reflection;
using Trafix;

namespace WpfApp_PropertyGridPractice
{
   public class MessageVM 
    {
        ENAppType m_appType;
        MessagesObservableCollection m_lstMessages = new MessagesObservableCollection();
        CustomMessage customMessage;
        protected CLRRuleEngine.IRuleSet m_ruleSetUpdateHeaderRules;
        protected CLRRuleEngine.IRuleSet m_ruleSetUpdateTrailerRules;
        protected CLRRuleEngine.IRuleSet m_ruleSetCondition;
        protected CLRRuleEngine.IRuleSet m_ruleSetUpdateRules;
        protected CLRRuleEngine.IRuleSet m_ruleSetUpdateGroupingRules;
        protected CLRRuleEngine.IRuleSet m_ruleSetUpdateGroupedRules;
        protected Dictionary<ENOrderEventType, CLRRuleEngine.IRuleSet> m_eventTypeRules;
        protected Dictionary<string, CLRRuleEngine.IRuleSet> m_rulesSetRecordConditions;
        protected Dictionary<string, CLRRuleEngine.IRuleSet> m_rulesSetRecordUpdateRules;
        public MessagesObservableCollection Messages
        {
            get
            {
                return m_lstMessages;
            }
            set
            {
                if (m_lstMessages != value)
                {
                    m_lstMessages = value;
                  //  RaisePropertyChanged("Messages");
                }
            }
        }

        public MessageVM(Object obj)
        {
            OnGetRecoveryMessages(obj);
        }

        private void OnGetRecoveryMessages(object obj)
        {
            object[] parametrs = obj as object[];
            string topic = parametrs[0] as string;
            if (string.IsNullOrEmpty(topic))
            {
             MessageBox.Show("Please select Firm Id.", "Error");
                return;
            }
            if (parametrs[1] == null)
            {
                MessageBox.Show("Please select App Type.", "Error");
                return;
            }
            m_appType=ENAppType.OMS;
            DateTime sDate = DateTime.MinValue;
            if (parametrs[2] is DateTime)
                sDate = (DateTime)parametrs[2];

            if (!string.IsNullOrEmpty(topic) )
            {
                PropertyDescriptorCollection collection = new PropertyDescriptorCollection(null, false);
                m_lstMessages.InitPropertyDescriptors();
                m_lstMessages.Clear();

                List<Message> lst = LoadDataFromLocalStore(topic);  //getting data from file
                m_lstMessages.SetPropertyDescription(GetPropertyDescriptors(collection, m_appType));
                if (lst != null)
                {
                    foreach (Message msg in lst)
                    {
                        CustomMessage msgDesc = new CustomMessage(msg, new MessageMetaDataProvider(GetFieldEnumType(m_appType)), 0, (int)msg.GetMessageType());
                        m_lstMessages.Add(msgDesc);
                    }
                }
            }
        }

        private PropertyDescriptorCollection GetPropertyDescriptors(PropertyDescriptorCollection collection, ENAppType appType)
        {
            // Property descriptor for message type                
            AddPropertyDescriptor(collection, appType, 0, null, "MessageType");

            if (appType == ENAppType.OMS)
            {
                List<ENField> omsFields = new List<ENField>();
                omsFields.Add(ENField.OrderId);
                omsFields.Add(ENField.Version);
                omsFields.Add(ENField.OMSTransType);
                omsFields.Add(ENField.Transaction);

                foreach (ENField val in omsFields)
                    AddPropertyDescriptor(collection, appType, (int)val, val, null);
            }
           
            return collection;
        }


        private void AddPropertyDescriptor(PropertyDescriptorCollection collection, ENAppType appType, int tag, object value, string name)
        {
            if (string.IsNullOrEmpty(name))
                name = Enum.GetName(typeof(Trafix.OMSCommon.ENField), value);

            MessageCollectionPropertyDescriptor pd = new MessageCollectionPropertyDescriptor(GetFieldEnumType(m_appType), tag, name);
            if (!collection.Contains(pd))
                collection.Add(pd);
        }


        public List<Message> LoadDataFromLocalStore(string topic, string error = null)
        {
            List<Message> lstMessages = null;
            //string storePath = ConfigurationManager.AppSettings["StoreFilePath"].ToString();
          //  string storePath = @"C:\Users\fnaqvi\Downloads\";
            string storePath = @"D:\Store\2022-03-16\";
            error = string.Empty;
            if (!string.IsNullOrEmpty(storePath))
            {
                Trafix.CLRPersistence.Persistence persistence = new Trafix.CLRPersistence.Persistence(4);
                //  string stDate = sDate.ToString("yyyy-MM-dd");
                //storePath = Path.Combine(storePath, stDate);
                if (persistence.Init(storePath, topic, false, false, "") == 0)
                {
                    // store file found
                    persistence.GetRecoveryData(out lstMessages);
                }
                else
                {
                    error = "Store file not found / Error loading store file";
                }
            }
            else
            {
                error = "Store file path not found";
            }
            return lstMessages;
        }

        private Type GetFieldEnumType(ENAppType appType)
        {
            //if (appType == ENAppType.ALLOCATION)
            //    return typeof(Trafix.Allocation.ENField);
            //else if (appType == ENAppType.TRS)
            //    return typeof(Trafix.TRSCommon.ENField);
            if (appType == ENAppType.OMS)
                return typeof(Trafix.OMSCommon.ENField);
            //else if (appType == ENAppType.RMS)
            //    return typeof(Trafix.RiskManagementServer.ENField);
            //else if (appType == ENAppType.IOI)
            //    return typeof(Trafix.IOI.ENField);

            return null;
        }

        public void initMsg(CustomMessage msg)
        {
             customMessage = msg;
           }

        protected virtual void ReadRules(string editorText, out string rulesCondition, out string rulesUpdate, out string rulesUpdateHeader, out string rulesUpdateTrailer, out string rulesUpdateGrouping, out string rulesUpdateGrouped, out string rulesListAndMap, out Dictionary<ENOrderEventType, string> eventTypeRulesString, out Dictionary<string, string> recordRulesCondition, out Dictionary<string, string> recordRulesUpdate)
        {
                rulesCondition = "";
                rulesUpdate = "";
                rulesUpdateHeader = "";
                rulesUpdateTrailer = "";
                rulesUpdateGrouping = "";
                rulesUpdateGrouped = "";
                rulesListAndMap = "";
                eventTypeRulesString = new Dictionary<ENOrderEventType, string>();
                recordRulesCondition = new Dictionary<string, string>();
                recordRulesUpdate = new Dictionary<string, string>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(editorText); // converting editor text/rules into xml format

                foreach (XmlNode childNode in xmlDoc.ChildNodes)
                {
                    if (childNode.Name.Equals("RulesEngine"))
                    {
                        foreach (XmlNode ruleEngineChild in childNode.ChildNodes)
                        {
                            if (ruleEngineChild.Name.Equals("RulesCondition"))
                            {
                                if (ruleEngineChild.Attributes != null && ruleEngineChild.Attributes["RecordName"] != null)
                                    recordRulesCondition[ruleEngineChild.Attributes["RecordName"].Value] = ruleEngineChild.InnerXml;
                                else
                                    rulesCondition = ruleEngineChild.InnerXml;
                            }
                            else if (ruleEngineChild.Name.Equals("RulesUpdate"))
                            {
                                if (ruleEngineChild.Attributes != null && ruleEngineChild.Attributes["RecordName"] != null)
                                    recordRulesUpdate[ruleEngineChild.Attributes["RecordName"].Value] = ruleEngineChild.InnerXml;
                                else
                                    rulesUpdate = ruleEngineChild.InnerXml;
                            }
                            else if (ruleEngineChild.Name.Equals("RulesUpdateHeader"))
                                rulesUpdateHeader = ruleEngineChild.InnerXml;
                            else if (ruleEngineChild.Name.Equals("RulesUpdateTrailer"))
                                rulesUpdateTrailer = ruleEngineChild.InnerXml;
                            else if (ruleEngineChild.Name.Equals("RulesUpdateGrouping"))
                                rulesUpdateGrouping = ruleEngineChild.InnerXml;
                            else if (ruleEngineChild.Name.Equals("RulesUpdateGrouped"))
                                rulesUpdateGrouped = ruleEngineChild.InnerXml;
                            else if (ruleEngineChild.Name.Equals("RulesListAndMap"))
                                rulesListAndMap = ruleEngineChild.InnerXml;
                            else if (ruleEngineChild.Name.StartsWith("RulesUpdate"))
                            {
                                ENOrderEventType eventType;
                                if (ENOrderEventType.TryParse(ruleEngineChild.Name.Substring(11), out eventType))
                                    eventTypeRulesString[eventType] = ruleEngineChild.InnerXml;
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Incorrect XML format!", "Error");
            }
              
        }


        public CustomMessage ApplyRules(string rules)
        {
            if (!string.IsNullOrEmpty(rules))
            {
                string stUpdateHeaderRule, stUpdateTrailerRule, stCondition, stUpdateRule, stUpdateGroupingRule, stUpdateGroupedRule, stListAndMapRule;
                Dictionary<string, string> recordRulesCondition = new Dictionary<string, string>();
                Dictionary<string, string> recordRulesUpdate;
                Dictionary<ENOrderEventType, string> eventTypeRules;
                ReadRules(rules, out stCondition, out stUpdateRule, out stUpdateHeaderRule, out stUpdateTrailerRule, out stUpdateGroupingRule, out stUpdateGroupedRule, out stListAndMapRule, out eventTypeRules, out recordRulesCondition, out recordRulesUpdate);

                var prop = customMessage.GetProperties();
                if (prop != null)
                {
                    setRules(prop, ref stCondition, ref stUpdateRule, ref stUpdateHeaderRule, ref stUpdateTrailerRule, ref stUpdateGroupingRule, ref stUpdateGroupedRule, ref stListAndMapRule, ref eventTypeRules, ref recordRulesCondition, ref recordRulesUpdate);

                    if (!string.IsNullOrEmpty(stCondition))
                    {
                        m_ruleSetCondition = CreateRuleSet(stCondition);
                        if (m_ruleSetCondition == null)
                            return null;
                    }
                    else
                        m_ruleSetCondition = null;

                    if (!string.IsNullOrEmpty(stUpdateRule))
                    {
                        m_ruleSetUpdateRules = CreateRuleSet(stUpdateRule);
                        if (m_ruleSetUpdateRules == null)
                            return null;
                    }
                    else
                        m_ruleSetUpdateRules = null;

                    if (!string.IsNullOrEmpty(stUpdateHeaderRule))
                    {
                        m_ruleSetUpdateHeaderRules = CreateRuleSet(stUpdateHeaderRule);
                        if (m_ruleSetUpdateHeaderRules == null)
                            return null;
                    }
                    else
                        m_ruleSetUpdateHeaderRules = null;


                    if (!string.IsNullOrEmpty(stUpdateTrailerRule))
                    {
                        m_ruleSetUpdateTrailerRules = CreateRuleSet(stUpdateTrailerRule);
                        if (m_ruleSetUpdateTrailerRules == null)
                            return null;
                    }
                    else
                        m_ruleSetUpdateTrailerRules = null;


                    if (!string.IsNullOrEmpty(stUpdateGroupingRule))
                    {
                        m_ruleSetUpdateGroupingRules = CreateRuleSet(stUpdateGroupingRule);
                        if (m_ruleSetUpdateGroupingRules == null)
                            return null;
                    }
                    else
                        m_ruleSetUpdateGroupingRules = null;


                    if (!string.IsNullOrEmpty(stUpdateGroupedRule))
                    {
                        m_ruleSetUpdateGroupedRules = CreateRuleSet(stUpdateGroupedRule);
                        if (m_ruleSetUpdateGroupedRules == null)
                            return null;
                    }
                    else
                        m_ruleSetUpdateGroupedRules = null;

                    FieldMap record = GenerateRecord(prop);
                    CustomMessage updatedMsg = new CustomMessage(record, new MessageMetaDataProvider(GetFieldEnumType(m_appType)), 0);

                    return updatedMsg;
                }
            }
            return null;
        }
      
       
        public void setRules(PropertyDescriptorCollection prop, ref string stCondition, ref string stUpdateRule, ref string stUpdateHeaderRule, ref string stUpdateTrailerRule, ref string stUpdateGroupingRule, ref string stUpdateGroupedRule, ref string stListAndMapRule, ref Dictionary<ENOrderEventType, string> eventTypeRules, ref Dictionary<string, string> recordRulesCondition, ref Dictionary<string, string> recordRulesUpdate)
        {
            RepeatingGroups group= null;
            int tag;
            for (int i = 0; i < prop.Count; i++)
            {
                var type = prop[i].GetValue("m_properties").GetType();
                if (type.Name == "RepeatingGroups")
                {
                    group = (RepeatingGroups)prop[i].GetValue("m_properties");
                   setRules(group.GetProperties(), ref stCondition, ref stUpdateRule, ref stUpdateHeaderRule, ref stUpdateTrailerRule, ref stUpdateGroupingRule, ref stUpdateGroupedRule, ref stListAndMapRule, ref eventTypeRules, ref recordRulesCondition, ref recordRulesUpdate);

                }
                else
                {
                    if (GetEnumValue(prop[i].Name) == 0)
                        tag = i + 1;
                    else
                        tag = GetEnumValue(prop[i].Name);

                      string stFieldToFind = "[" + prop[i].Name + "]";

                        if (stCondition.Contains(stFieldToFind))
                            stCondition = stCondition.Replace(stFieldToFind, tag.ToString());
                        if (stUpdateRule.Contains(stFieldToFind))
                            stUpdateRule = stUpdateRule.Replace(stFieldToFind, tag.ToString());
                        if (stUpdateHeaderRule.Contains(stFieldToFind))
                            stUpdateHeaderRule = stUpdateHeaderRule.Replace(stFieldToFind, tag.ToString());
                        if (stUpdateTrailerRule.Contains(stFieldToFind))
                            stUpdateTrailerRule = stUpdateTrailerRule.Replace(stFieldToFind, tag.ToString());
                        if (stUpdateGroupingRule.Contains(stFieldToFind))
                            stUpdateGroupingRule = stUpdateGroupingRule.Replace(stFieldToFind, tag.ToString());
                        if (stUpdateGroupedRule.Contains(stFieldToFind))
                            stUpdateGroupedRule = stUpdateGroupedRule.Replace(stFieldToFind, tag.ToString());
                }
            } 
         }

        virtual protected FieldMap GenerateRecord(PropertyDescriptorCollection prop)
        {
            FieldMap record = new FieldMap();
            FieldMap groupRecord = new FieldMap();
            RepeatingGroups collection = null;
            int position;

            for (int i = 0; i < prop.Count; i++)
            {
                var type = prop[i].GetValue("m_properties").GetType();

                if (type.Name == "RepeatingGroups")
                {
                    collection = (RepeatingGroups)prop[i].GetValue("m_properties");
                    groupRecord = GenerateRecord(collection.GetProperties());
                  
                    if (GetEnumValue(prop[i].Name) == 0)
                        position = i + 1;
                    else
                        position = GetEnumValue(prop[i].Name);
                    record.SetField((uint)position, groupRecord);
                }
                else
                {
                    var fieldValue = prop[i].GetValue("m_value");

                    if (fieldValue != null)
                    {
                       if (GetEnumValue(prop[i].Name) == 0)
                            position = i + 1;
                        else
                            position = GetEnumValue(prop[i].Name);
                           record.SetField((uint)position, fieldValue.ToString());
                    }
                }
            }

            if (m_ruleSetUpdateRules != null)
            {
                // Apply Update Rules using Rules Engine
                m_ruleSetUpdateRules.apply(record);
            }

            FieldMap finalRecord = getDataType(prop, record);
            return finalRecord;
            //return record;
        }

        public FieldMap getDataType(PropertyDescriptorCollection prop , FieldMap record)
        {
            FieldMap newRecord = new FieldMap();
            int position;
            for (int i = 0; i < prop.Count; i++)
            {

                if (GetEnumValue(prop[i].Name) == 0)
                    position = i + 1;
                else
                    position = GetEnumValue(prop[i].Name);

                if (record.TagExists((uint)position))
                {
                    record.GetField((uint)position, out string val);

                    char[] delims = new[] { ']', '[' };
                    var Nodes = prop[i].DisplayName.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                    if (Nodes[Nodes.Count() - 1].Trim() == "MESSAGE")
                    {

                        record.GetField((uint)position, out FieldMap msg);
                        newRecord.SetField((uint)position, msg);
                    }
                    else if (Nodes[Nodes.Count() - 1].Trim() == "DOUBLE")
                    {

                        double.TryParse(val, out double result);
                        newRecord.SetField((uint)position, result);
                    }
                    else if (Nodes[Nodes.Count() - 1].Trim() == "BOOL")
                    {

                        bool.TryParse(val, out bool result);
                        newRecord.SetField((uint)position, result);
                    }
                    else if (Nodes[Nodes.Count() - 1].Trim() == "INT" || Nodes[Nodes.Count() - 1].Trim() == "LONG")
                    {
                        Int32.TryParse(val, out int result);
                        newRecord.SetField((uint)position, result);
                    }
                    else
                        newRecord.SetField((uint)position, val.ToString());
                }
            }
            //if (newRecord.Count() < record.Count())
            //{
                foreach(var rec in record)
                {
                    if (!newRecord.TagExists(rec.Key))
                        newRecord.SetField(rec.Key, rec.Value.ToString());
                }
            //}
            

            return newRecord;
            } 

            //public FieldMap getDataType(PropertyDescriptorCollection prop , FieldMap record)
        //{
        //    FieldMap newRecord = new FieldMap();
        //    int position;
        //    for (int i = 0; i < prop.Count; i++)
        //    {

        //        if (GetEnumValue(prop[i].Name) == 0)
        //            position = i + 1;
        //        else
        //            position = GetEnumValue(prop[i].Name);
             
        //        record.GetField((uint)position, out string val);

        //        char[] delims = new[] { ']', '[' };
        //        var Nodes = prop[i].DisplayName.Split(delims, StringSplitOptions.RemoveEmptyEntries);

        //        if (Nodes[Nodes.Count()-1].Trim() == "MESSAGE")
        //        {

        //            record.GetField((uint)position, out FieldMap msg);
        //            newRecord.SetField((uint)position, msg);
        //        }
        //        else if (Nodes[Nodes.Count() - 1].Trim() == "DOUBLE")
        //        {
                    
        //            double.TryParse(val, out double result);
        //            newRecord.SetField((uint)position, result);
        //        }
        //        else if (Nodes[Nodes.Count() - 1].Trim() == "BOOL")
        //        {
                    
        //            bool.TryParse(val, out bool result);
        //            newRecord.SetField((uint)position, result);
        //        }
        //        else if (Nodes[Nodes.Count() - 1].Trim() == "INT" || Nodes[Nodes.Count() - 1].Trim() == "LONG")
        //        {
        //            Int32.TryParse(val, out int result);
        //            newRecord.SetField((uint)position, result);
        //        }
        //        else
        //            newRecord.SetField((uint)position, val.ToString());
        //    }

        //    if (newRecord.Count() < record.Count())
        //    {
        //        foreach(var rec in record)
        //        {
        //            if (!newRecord.TagExists(rec.Key))
        //                newRecord.SetField(rec.Key, rec.Value.ToString());
        //        }
        //    }
        //    else if (newRecord.Count() > record.Count())
        //    {
        //        foreach (var rec in newRecord)
        //        {
        //            if (!record.TagExists(rec.Key))
        //            {
        //                newRecord.Remove(rec.Key);
        //                continue;
        //            }
        //        }
        //    }

        //    return newRecord;
        //}

        protected CLRRuleEngine.IRuleSet CreateRuleSet(string stRules)
        {
             CLRRuleEngine.IRuleSet ruleSet = CLRRuleEngine.RuleEngine.createRuleSet(stRules);
                if (ruleSet == null)
                    return null;
            return ruleSet;
            }

        int GetEnumValue(string Name)
        {
           ENField.TryParse<ENField>(Name, out ENField field);

            return (int)field;
           
        }
   }
}
