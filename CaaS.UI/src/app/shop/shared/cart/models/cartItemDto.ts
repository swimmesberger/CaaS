import { DiscountDto } from './discountDto';
import {ProductMinimalDto} from "../../product/models/productMinimalDto";


export interface CartItemDto {
    id?: string;
    product?: ProductMinimalDto;
    shopId?: string;
    cartId?: string;
    amount?: number;
    cartItemDiscounts?: Array<DiscountDto> | null;
    totalPrice?: number;
    concurrencyToken?: string | null;
}

