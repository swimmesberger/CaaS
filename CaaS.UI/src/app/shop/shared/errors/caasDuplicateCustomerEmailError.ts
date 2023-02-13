export class CaasDuplicateCustomerEmailError extends Error {
  constructor(public msg: string) {
    super(msg);
  }
}
