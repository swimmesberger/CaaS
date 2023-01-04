import {AddressDto} from "./addressDto";
import {CustomerDto} from "../../cart/models/customerDto";

export interface CustomerWithAddressDto {
    customer?: CustomerDto;
    creditCard?: CreditCardDataDto;
    address?: AddressDto;
}

export interface CreditCardDataDto {
  creditCardNumber: string,
  fullName: string,
  validDate: string,
  cvc: string
}

