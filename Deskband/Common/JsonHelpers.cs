using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Common
{
    public static class JsonHelpers
    {
        public static void Merge(JContainer receiver, JContainer donor)
        {
            var receiverObject = receiver as JObject;
            var donorObject = donor as JObject;

            var receiverArray = receiver as JArray;
            var donorArray = donor as JArray;

            if (receiverObject != null && donorObject != null)
            {
                foreach (var property in donorObject)
                {
                    var receiverObjectValue = receiverObject[property.Key] as JObject;
                    var donorObjectValue = property.Value as JObject;

                    var receiverArrayValue = receiverObject[property.Key] as JArray;
                    var donorArrayValue = property.Value as JArray;

                    if (receiverObjectValue != null && donorObjectValue != null)
                        Merge(receiverObjectValue, donorObjectValue);
                    else if (receiverArrayValue != null && donorArrayValue != null)
                        Merge(receiverArrayValue, donorArrayValue);
                    else
                        receiverObject[property.Key] = property.Value;
                }
            }
            else if (receiverArray != null && donorArray != null)
            {
                for (int i = 0; i < donorArray.Count; i++)
                {
                    var receiverArrayValue = i < receiverArray.Count ? receiverArray[i] as JArray : null;
                    var donorArrayValue = donorArray[i] as JArray;

                    var receiverObjectValue = i < receiverArray.Count ? receiverArray[i] as JObject : null;
                    var donorObjectValue = donorArray[i] as JObject;

                    if (receiverArrayValue != null && donorArrayValue != null)
                        Merge(receiverArrayValue, donorArrayValue);
                    else if (receiverObjectValue != null && donorObjectValue != null)
                        Merge(receiverObjectValue, donorObjectValue);
                    else
                    {
                        if (i < receiverArray.Count)
                            receiverArray[i] = donorArray[i];
                        else
                            receiverArray.Add(donorArray[i]);
                    }
                }
            }
        }

        public static T CloneObject<T>(T target)
        {
            string json = JsonConvert.SerializeObject(target);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}