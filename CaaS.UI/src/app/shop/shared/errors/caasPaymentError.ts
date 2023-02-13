export class CaasPaymentError extends Error {
  constructor(public msg: string) {
    super(msg);
  }
}
