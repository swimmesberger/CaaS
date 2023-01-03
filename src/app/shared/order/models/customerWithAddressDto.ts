import {AddressDto} from "./addressDto";
import {CustomerDto} from "../../cart/models/customerDto";

export interface CustomerWithAddressDto {
    customer?: CustomerDto;
    address?: AddressDto;
}

