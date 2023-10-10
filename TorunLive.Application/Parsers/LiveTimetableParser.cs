using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Application.Interfaces;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Parsers
{
    public class LiveTimetableParser : ILiveTimetableParser
    {
        // update content
        private static string Example = @"1|#||4|53|updatePanel|ctl00_ctl00_ContentPlaceHolderContenido_UpdatePanelTime|
                        00:11
                    |2236|updatePanel|ctl00_ctl00_ContentPlaceHolderContenido_UpdatePanel1|
                        
    
            <table class=""tablePanel"" style=""width: 100%;"">
                <thead>
                    <tr>
                        <th scope=""col"" style=""width: 50px;"">
                            Linia
                        </th>
                        
                        <th scope=""col"">
                            Kierunek
                        </th>
                        
                        <th scope=""col"" style=""width: 60px;"">
                            Odjazd
                        </th>
                        
                    </tr>
                </thead>
                <tbody>
        
            <tr>
                <td id=""ctl00_ctl00_ContentPlaceHolderContenido_ContentPlaceHolderContenido_RepeaterLCDEntries_ctl01_tdsLinea"" style=""text-align:center;background-color:#F00000;color:#FFFFFF;"">
                    N90
                </td>

                
                <td>
                    UNIWERSYTET
                
                        <span style=""float:right;""><img src=""../../images/wheelchair-24.png"" alt=""Wheelchair"" style=""height:16px;border-width:0px;display:block;"" /></span>         
                
                </td>
                
                <td style=""text-align: center"">
                    24min
                </td>
                
            </tr>
        
            <tr>
                <td id=""ctl00_ctl00_ContentPlaceHolderContenido_ContentPlaceHolderContenido_RepeaterLCDEntries_ctl02_tdsLinea"" style=""text-align:center;background-color:#F00000;color:#FFFFFF;"">
                    N90
                </td>

                
                <td>
                    MOTOARENA
                
                        <span style=""float:right;display:none""><img src=""../../images/wheelchair-24.png"" alt=""Wheelchair"" style=""height:16px;border-width:0px;display:block;"" /></span>         
                
                </td>
                
                <td style=""text-align: center"">
                    4:39
                </td>
                
            </tr>
        
            </tbody> </table>
        

                    |0|hiddenField|__EVENTTARGET||0|hiddenField|__EVENTARGUMENT||1948|hiddenField|__VIEWSTATE|/wEPDwUKLTg2ODI2OTMyNw9kFgJmD2QWAmYPZBYEAgEPZBYCAg8PFgIeBGhyZWYFP34vY3NzL2N1c3RvbVN0eWxlLmFzcHg/dG1wPWRlMDE1MGQ3LWJiZDktNDdlZi04ODA5LWM0ZmMzOWViZjAyNWQCAw9kFgwCAQ9kFhACEA8PFgIeB1Zpc2libGVoZGQCEQ8PFgIfAWhkZAISDw8WAh8BaGRkAhMPDxYCHwFoZGQCFA8PFgIfAWhkZAIVDw8WAh8BaGRkAhYPDxYCHwFoZGQCGA9kFgJmDxYCHgRUZXh0BTFUT1JVxYMgU2Vyd2lzIHRyYW5zcG9ydCBwdWJsaWN6bnkgU3Ryb25hIEfFgsOzd25hZAIED2QWAgIHDxAPFgIfAWhkZBYAZAIGDxYCHwFoFgQCAQ8WAh8CBRpOYXp3YSBvYnN6YXJ1IHphdHJ6eW1hbmlhOmQCAw8QZGQWAGQCBw9kFgICBQ8QDxYCHwFoZGQWAGQCDA9kFgQCAw8QZBAVAhBXeWJpZXJ6IHVsdWJpb25lGiBOYXp3YSBwcnp5c3Rhbmt1OiBPZCBOb3dhFQICLTEBMBQrAwJnZ2RkAgkPFgIfAgUBMWQCDQ9kFgxmDxYCHwIFDzY0MzAxIC0gT2QgTm93YWQCAQ9kFgJmD2QWAgIBDxYCHwIFBTAwOjExZAICDxYEHghJbnRlcnZhbAKgnAEeB0VuYWJsZWRnZAIDD2QWAmYPZBYCAgEPZBYCAgEPFgIeC18hSXRlbUNvdW50AgIWBAIBD2QWEGYPFgIeBXN0eWxlBTl0ZXh0LWFsaWduOmNlbnRlcjtiYWNrZ3JvdW5kLWNvbG9yOiNGMDAwMDA7Y29sb3I6I0ZGRkZGRjsWAmYPFQEDTjkwZAIBDxUBBDA6MDBkAgIPFQEEMDozNWQCAw8VAQtVTklXRVJTWVRFVGQCBA8VAQBkAgYPFQEFMjRtaW5kAgcPFQEFMjRtaW5kAgkPFQEAZAICD2QWEGYPFgIfBgU5dGV4dC1hbGlnbjpjZW50ZXI7YmFja2dyb3VuZC1jb2xvcjojRjAwMDAwO2NvbG9yOiNGRkZGRkY7FgJmDxUBA045MGQCAQ8VAQQ0OjEwZAICDxUBBDQ6MzlkAgMPFQEJTU9UT0FSRU5BZAIEDxUBDGRpc3BsYXk6bm9uZWQCBg8VAQQ0OjM5ZAIHDxUBBDQ6MzlkAgkPFQEAZAIFD2QWBAIBDxYCHwFoZAIRDxYCHwFoZAIGD2QWBgIDDxYCHwFoZAIFDxAPFgIfAWhkZBYAZAIJDxAPFgIeC18hRGF0YUJvdW5kZ2RkZGQYAwUeX19Db250cm9sc1JlcXVpcmVQb3N0QmFja0tleV9fFg4FEGN0bDAwJGN0bDAwJGliRjEFEGN0bDAwJGN0bDAwJGliRjIFEGN0bDAwJGN0bDAwJGliRjMFEGN0bDAwJGN0bDAwJGliRjQFFmN0bDAwJGN0bDAwJGliRXNxdWVtYTEFFmN0bDAwJGN0bDAwJGliRXNxdWVtYTIFFmN0bDAwJGN0bDAwJGliRXNxdWVtYTMFFmN0bDAwJGN0bDAwJGliRXNxdWVtYTQFEGN0bDAwJGN0bDAwJGliUGwFEGN0bDAwJGN0bDAwJGliRGUFEGN0bDAwJGN0bDAwJGliRW4FEGN0bDAwJGN0bDAwJGlkRXMFLmN0bDAwJGN0bDAwJExvZ2luVmlldzEkTG9naW5TdGF0dXNMb2dnZWQkY3RsMDEFLmN0bDAwJGN0bDAwJExvZ2luVmlldzEkTG9naW5TdGF0dXNMb2dnZWQkY3RsMDMFFmN0bDAwJGN0bDAwJExvZ2luVmlldzEPD2QCAWQFFmN0bDAwJGN0bDAwJExvZ2luVmlldzIPD2QCAWQggCEXU68/WmiiuJvedTmJgcildjbsm9gMv0pZ2Qq5LQ==|8|hiddenField|__VIEWSTATEGENERATOR|D63B2DD0|1160|hiddenField|__EVENTVALIDATION|/wEdADTKDlleY7gd4p4ox7DfJLCDpUfIgJTaaE5eDeJIhdE43cbkZjuiGz/vt49v3/cW3Fs5W5m5MuiYAQ1Bi59M+gIWLqzW5asF56HY6tvh2wyXzLnh6fU4hI1DWPNBR8AbV75+jbXTEaQYuR901vC1LWU9vOfCeAh9cLGujitnWLzS+Xp1cFZ96aFDV31v/lR0DTcdXJoU9Fe8Ki8vwmNfujZIM7bNQrOtHR5j2tfEfGSlGwT0qjiEHrQhRA5yxfPdWQYK8vYdNXPffGCz6bDJhdO7ks85m/DkcE8FrZaz6gs2Zye2+YY6aO3fFO3De/afMuz0biIosZOaGOdo/vy1e4MRZKm1ZHg6mB9kFMJYo3cCD4g66Xl5qTOBVGn9dB0NBQhyx1AeAhsprghMoqa2PrFrsyXMEbzkPnSgMWnNtRUihbyBEos7i7R3XpM2tEOoMwM6wx9BNORRZloWJDjrRYkiMIZM6oVjLTmSByzmJriuJdjDc7PADgOei6GFSGSjXTEyImDbXzJOnGmaC0Hk0GajxPKjWY/OuRL6XX2S1W+JwW0knh/4/IACTC5TpZSiV+eL8GKvESNZcHUPPzeobawM3Ua/Rmgm/Q1FimypxQzeXPVrl4WYChbGCVMDCtqRkM4i6BZcn9PbFQToEJLN7uEfseqJGRj/AWRH2wrn10GHukALT/kk1RE1pZNZBbdq/JIgow5vyML/giMoGucrPyc/pMERvtVSANbbofXITGqqfY3OatxA9u/UZ8n3MyH7E0xZWtNW448P6tzEBXvsoVxH+FiDohHmQXa177+ShhQ8fUAVpkquUS3ZeP9U5RcFS2CbVKhJWlSttQWFPqrLLwx99tHkaQBQd/MmwjyMOrj5/2NSy+y4YIHouHWVTYW9NzTPs1u8qGIsiezwSOwKk8GwCdJ5LMCCTUvQH62EJVtHh351b6BDAMy0hf5Op8ttPNAcizdhIcW573xxk9cRS+T0MN/NTVd5GszMUuCXjf8J/Am+jBhyUeYXRsOUtnYp/xxze6qwydnGC4+7RxioO5F7/KVDDgazHgmoc0+lVP3E5PdGr+pO5Yrd4FDJFNlPix0smENSlZZdh3nO7+xPP5r22qWBCb50kmRMzlsIUzrRs2jGdUHNuZksSIv6JeKM2wE=|47|asyncPostBackControlIDs||ctl00$ctl00$ContentPlaceHolderContenido$Timer1,|0|postBackControlIDs|||112|updatePanelIDs||tctl00$ctl00$ContentPlaceHolderContenido$UpdatePanelTime,,tctl00$ctl00$ContentPlaceHolderContenido$UpdatePanel1,|0|childUpdatePanelIDs|||110|panelsToRefreshIDs||ctl00$ctl00$ContentPlaceHolderContenido$UpdatePanelTime,,ctl00$ctl00$ContentPlaceHolderContenido$UpdatePanel1,|2|asyncPostBackTimeout||90|25|formAction||./default.aspx?stop=64301|192|scriptBlock|ScriptPath|/ScriptResource.axd?d=-fhkJShfZMlSEKsXelOwbXoFVs0PO76_9qH1xD9vLRNdtpiH-4p3OXlN1gaCNYovsleMdCDsOM58MO6_QwiFLIcsqsc4EgLTxsb6n-8iJtAUCcY6zTGn95XxeSISgIl4ECPmEgPdrarKvh-XnYcAuA2&t=ffffffff93f2983c|9|focus||btnSearch|";
        public LiveTimetable Parse(string data)
        {
            //data = Example;
            //var panelName = "ctl00_ctl00_ContentPlaceHolderContenido_UpdatePanel1|";
            //var endMarker = "|0|hiddenField";
            var panelName = "<table class=\"tablePanel\"";
            var startIndex = data.IndexOf(panelName);
            var endMarker = "</table>";
            var endIndex = data.IndexOf(endMarker);
            var substring = data.Substring(startIndex, endIndex + endMarker.Length - startIndex);

            var document = XDocument.Parse(substring);
            var arrivals = document.XPathSelectElements("table/tbody/tr");
            var liveArrivals = new List<LiveArrival>();
            foreach (var arrival in arrivals)
            {
                var cells = arrival.XPathSelectElements("td").ToList();
                if (cells.Count != 3)
                {
                    Console.WriteLine("Cell count is not 3!");
                }
                var lineNumber = cells[0].Value.Trim();
                var lineName = cells[1].Value.Trim();
                var time = cells[2].Value.Trim();
                liveArrivals.Add(new LiveArrival
                {
                    Number = lineNumber,
                    Name = lineName,
                    DayMinute = ConvertArrivalTimeToDayMinute(time)
                });
            }

            // possible two entries of the same line
            var lines = liveArrivals.GroupBy(k => k.Number)
                .Select(k =>
                {
                    var arrival = k.First();
                    return new Line
                    {
                        Name = arrival.Name,
                        Number = arrival.Number,
                        Arrivals = k.Select(la => new Arrival { DayMinute = la.DayMinute }).ToList()
                    };
                }).ToList();

            return new LiveTimetable { Lines = lines };
        }

        private static int ConvertArrivalTimeToDayMinute(string minutesOrHourMinute)
        {
            if (minutesOrHourMinute == ">>")
            {
                var now = DateTime.Now;
                var dayMinute = now.Hour * 60 + now.Minute;
                return dayMinute;
            }

            if (minutesOrHourMinute.Contains("min"))
            {
                var value = int.Parse(minutesOrHourMinute.Replace("min", ""));
                var arrivalDateTime = DateTime.Now.AddMinutes(value);
                var dayMinute = arrivalDateTime.Hour * 60 + arrivalDateTime.Minute;
                return dayMinute;
            }
            else
            {
                var hourAndMinute = minutesOrHourMinute.Split(':');
                var hour = int.Parse(hourAndMinute[0]);
                var minute = int.Parse(hourAndMinute[1]);
                var dayMinute = hour * 60 + minute;
                return dayMinute;
            }
        }
    }
}
