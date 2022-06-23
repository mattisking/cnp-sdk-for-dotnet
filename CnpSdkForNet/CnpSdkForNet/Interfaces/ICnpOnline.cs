using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cnp.Sdk.Interfaces
{
    // CnpOnline interface for synchronous and asynchronous call.
    public interface ICnpOnline
    {
        authorizationResponse Authorize(authorization auth);
        Task<authorizationResponse> AuthorizeAsync(authorization auth, CancellationToken cancellationToken);
        authReversalResponse AuthReversal(authReversal reversal);
        Task<authReversalResponse> AuthReversalAsync(authReversal reversal, CancellationToken cancellationToken);
        captureResponse Capture(capture capture);
        Task<captureResponse> CaptureAsync(capture capture, CancellationToken cancellationToken);
        captureGivenAuthResponse CaptureGivenAuth(captureGivenAuth captureGivenAuth);
        Task<captureGivenAuthResponse> CaptureGivenAuthAsync(captureGivenAuth captureGivenAuth, CancellationToken cancellationToken);
        creditResponse Credit(credit credit);
        Task<creditResponse> CreditAsync(credit credit, CancellationToken cancellationToken);
        echeckCreditResponse EcheckCredit(echeckCredit echeckCredit);
        Task<echeckCreditResponse> EcheckCreditAsync(echeckCredit echeckCredit, CancellationToken cancellationToken);
        echeckRedepositResponse EcheckRedeposit(echeckRedeposit echeckRedeposit);
        Task<echeckRedepositResponse> EcheckRedepositAsync(echeckRedeposit echeckRedeposit, CancellationToken cancellationToken);
        echeckSalesResponse EcheckSale(echeckSale echeckSale);
        Task<echeckSalesResponse> EcheckSaleAsync(echeckSale echeckSale, CancellationToken cancellationToken);
        echeckVerificationResponse EcheckVerification(echeckVerification echeckVerification);
        Task<echeckVerificationResponse> EcheckVerificationAsync(echeckVerification echeckVerification, CancellationToken cancellationToken);
        forceCaptureResponse ForceCapture(forceCapture forceCapture);
        Task<forceCaptureResponse> ForceCaptureAsync(forceCapture forceCapture, CancellationToken cancellationToken);
        saleResponse Sale(sale sale);
        Task<saleResponse> SaleAsync(sale sale, CancellationToken cancellationToken);
        registerTokenResponse RegisterToken(registerTokenRequestType tokenRequest);
        Task<registerTokenResponse> RegisterTokenAsync(registerTokenRequestType tokenRequest, CancellationToken cancellationToken);
        voidResponse DoVoid(voidTxn v);
        Task<voidResponse> DoVoidAsync(voidTxn v, CancellationToken cancellationToken);
        echeckVoidResponse EcheckVoid(echeckVoid v);
        Task<echeckVoidResponse> EcheckVoidAsync(echeckVoid v, CancellationToken cancellationToken);
        updateCardValidationNumOnTokenResponse UpdateCardValidationNumOnToken(updateCardValidationNumOnToken update);
        Task<updateCardValidationNumOnTokenResponse> UpdateCardValidationNumOnTokenAsync(updateCardValidationNumOnToken update, CancellationToken cancellationToken);
        giftCardAuthReversalResponse GiftCardAuthReversal(giftCardAuthReversal giftCard);
        Task<giftCardAuthReversalResponse> GiftCardAuthReversalAsync(giftCardAuthReversal giftCard, CancellationToken cancellationToken);
        giftCardCaptureResponse GiftCardCapture(giftCardCapture giftCardCapture);
        Task<giftCardCaptureResponse> GiftCardCaptureAsync(giftCardCapture giftCardCapture, CancellationToken cancellationToken);

        payFacCreditResponse PayFacCredit(payFacCredit payFacCredit);
        Task<payFacCreditResponse> PayFacCreditAsync(payFacCredit payFacCredit, CancellationToken cancellationToken);
        payFacDebitResponse PayFacDebit(payFacDebit payFacDebit);
        Task<payFacDebitResponse> PayFacDebitAsync(payFacDebit payFacDebit, CancellationToken cancellationToken);

        physicalCheckCreditResponse PhysicalCheckCredit(physicalCheckCredit physicalCheckCredit);
        Task<physicalCheckCreditResponse> PhysicalCheckCreditAsync(physicalCheckCredit physicalCheckCredit, CancellationToken cancellationToken);
        physicalCheckDebitResponse PhysicalCheckDebit(physicalCheckDebit physicalCheckDebit);
        Task<physicalCheckDebitResponse> PhysicalCheckDebitAsync(physicalCheckDebit physicalCheckDebit, CancellationToken cancellationToken);

        payoutOrgCreditResponse PayoutOrgCredit(payoutOrgCredit payoutOrgCredit);
        Task<payoutOrgCreditResponse> PayoutOrgCreditAsync(payoutOrgCredit payoutOrgCredit, CancellationToken cancellationToken);
        payoutOrgDebitResponse PayoutOrgDebit(payoutOrgDebit payoutOrgDebit);
        Task<payoutOrgDebitResponse> PayoutOrgDebitAsync(payoutOrgDebit payoutOrgDebit, CancellationToken cancellationToken);


        reserveCreditResponse ReserveCredit(reserveCredit reserveCredit);
        Task<reserveCreditResponse> ReserveCreditAsync(reserveCredit reserveCredit, CancellationToken cancellationToken);
        reserveDebitResponse ReserveDebit(reserveDebit reserveDebit);
        Task<reserveDebitResponse> ReserveDebitAsync(reserveDebit reserveDebit, CancellationToken cancellationToken);

        submerchantCreditResponse SubmerchantCredit(submerchantCredit submerchantCredit);
        Task<submerchantCreditResponse> SubmerchantCreditAsync(submerchantCredit submerchantCredit, CancellationToken cancellationToken);
        submerchantDebitResponse SubmerchantDebit(submerchantDebit submerchantDebit);
        Task<submerchantDebitResponse> SubmerchantDebitAsync(submerchantDebit submerchantDebit, CancellationToken cancellationToken);

        vendorCreditResponse VendorCredit(vendorCredit vendorCredit);
        Task<vendorCreditResponse> VendorCreditAsync(vendorCredit vendorCredit, CancellationToken cancellationToken);
        vendorDebitResponse VendorDebit(vendorDebit vendorDebit);
        Task<vendorDebitResponse> VendorDebitAsync(vendorDebit vendorDebit, CancellationToken cancellationToken);

        customerCreditResponse CustomerCredit(customerCredit customerCredit);
        Task<customerCreditResponse> CustomerCreditAsync(customerCredit customerCredit, CancellationToken cancellationToken);
        customerDebitResponse CustomerDebit(customerDebit customerDebit);
        Task<customerDebitResponse> CustomerDebitAsync(customerDebit customerDebit, CancellationToken cancellationToken);

        giftCardCreditResponse GiftCardCredit(giftCardCredit giftCardCredit);
        Task<giftCardCreditResponse> GiftCardCreditAsync(giftCardCredit giftCardCredit, CancellationToken cancellationToken);
        transactionTypeWithReportGroup QueryTransaction(queryTransaction queryTransaction);
        Task<transactionTypeWithReportGroup> QueryTransactionAsync(queryTransaction queryTransaction, CancellationToken cancellationToken);
        fraudCheckResponse FraudCheck(fraudCheck fraudCheck);
        Task<fraudCheckResponse> FraudCheckAsync(fraudCheck fraudCheck, CancellationToken cancellationToken);

        translateToLowValueTokenResponse TranslateToLowValueTokenRequest(translateToLowValueTokenRequest translateToLowValueTokenRequest);
        Task<translateToLowValueTokenResponse> TranslateToLowValueTokenRequestAsync(translateToLowValueTokenRequest translateToLowValueTokenRequest, CancellationToken cancellationToken);


        event EventHandler HttpAction;
    }
}
