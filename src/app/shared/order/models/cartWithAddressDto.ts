import {CartDto} from "../../cart/models/cartDto";
import {AddressDto} from "./addressDto";

export interface CartWithAddressDto {
    cart?: CartDto;
    address?: AddressDto;
}

