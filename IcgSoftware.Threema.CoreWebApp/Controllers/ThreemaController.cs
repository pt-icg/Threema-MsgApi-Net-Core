using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IcgSoftware.Threema.CoreMsgApi;
using IcgSoftware.Threema.CoreMsgApi.Messages;
using IcgSoftware.Threema.CoreMsgApi.Helpers;
using IcgSoftware.Threema.CoreWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IcgSoftware.Threema.CoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreemaController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ILogger<ThreemaController> _log;
        private IHostingEnvironment _hostingEnvironment;

        public ThreemaController(IConfiguration configuration, ILogger<ThreemaController> log, IHostingEnvironment hostingEnvironment)
        {
            this._configuration = configuration;
            this._log = log;
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("[action]")]
        public bool MessageCallback([FromForm] ThreemaMessageCallback threemaMessageCallback) 
        {
            try
            {
                try
                {
                    _log.LogDebug("ThreemaController.MessageCallback: ModelState {0}, ThreemaMessageCallback {1}", ModelState.IsValid, threemaMessageCallback.ToString());
                }
                catch (Exception ex)
                {
                    _log.LogError("ThreemaController.MessageCallback 1 " + ex.Message);
                }

                string nonce = threemaMessageCallback.Nonce;
                string box = threemaMessageCallback.Box;

                string callbackPublicKey = GetPublicKey(threemaMessageCallback.From);

                string myPrivateKey = this._configuration["Threema:PrivateKey"];
                Key privateKey = Key.DecodeKey(myPrivateKey);
                Key publicKey = Key.DecodeKey(callbackPublicKey);

                ThreemaMessage message = CryptTool.DecryptMessage(
                        DataUtils.HexStringToByteArray(box),
                        privateKey.key,
                        publicKey.key,
                        DataUtils.HexStringToByteArray(nonce)
                );

                switch (message.GetTypeCode())
                {
                    case TextMessage.TYPE_CODE:
                        _log.LogInformation("ThreemaController.MessageCallback TextMessage from {0} to {1}: {2}", threemaMessageCallback.From, threemaMessageCallback.To, message.ToString());
                        break;
                    //case DeliveryReceipt.TYPE_CODE:
                    //class DeliveryReceipt statt public class DeliveryReceipt
                    case 0x80:
                        _log.LogInformation("ThreemaController.MessageCallback DeliveryReceipt from {0} to {1}: {2}", threemaMessageCallback.From, threemaMessageCallback.To, message.ToString());
                        break;
                    default:
                        _log.LogInformation("ThreemaController.MessageCallback ThreemaMessage? from {0} to {1}: {2}", threemaMessageCallback.From, threemaMessageCallback.To, message.ToString());
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError("ThreemaController.MessageCallback 2 " + ex.Message);
                return false;
            }
        }

        private string GetPublicKey(string threemaId)
        {
            string myThreemaId = this._configuration["Threema:ThreemaId"];
            string mySecret = this._configuration["Threema:Secret"];
            APIConnector apiConnector = new APIConnector(myThreemaId, mySecret, null);
            byte[] publicKey = apiConnector.LookupKey(threemaId);
            if (publicKey != null)
            {
                string publicKeyEncoded = new Key(Key.KeyType.PUBLIC, publicKey).Encode();
                return publicKeyEncoded;
            }
            else
            {
                throw new Exception(String.Format("public key for {0} not found", threemaId));
            }
        }

        [HttpPost("[action]")]
        public bool SendE2ETextMessage(ThreemaE2ETextMessage threemaE2ETextMessage)
        {
            try
            {
                try
                {
                    _log.LogDebug("ThreemaController.SendE2ETextMessage: ModelState {0}, ThreemaE2ETextMessage {1}", ModelState.IsValid, threemaE2ETextMessage.ToString());

                }
                catch (Exception ex)
                {
                    _log.LogError("ThreemaController.SendE2ETextMessage 1 " + ex.Message);
                }

                string myPrivateKey = this._configuration["Threema:PrivateKey"];
                string myThreemaId = this._configuration["Threema:ThreemaId"];
                string mySecret = this._configuration["Threema:Secret"];
                APIConnector apiConnector = new APIConnector(myThreemaId, mySecret, null);
                string text = DataUtils.Utf8Endcode(threemaE2ETextMessage.Text.Trim());
                Key privateKey = Key.DecodeKey(myPrivateKey);
                E2EHelper e2EHelper = new E2EHelper(apiConnector, privateKey.key);
                String messageId = e2EHelper.SendTextMessage(threemaE2ETextMessage.To, text);
                _log.LogDebug("ThreemaController.SendE2ETextMessage: messege sendet to {0}", threemaE2ETextMessage.To);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError("ThreemaController.SendE2ETextMessage 2 " + ex.Message);
                return false;
            }
        }


    }
}