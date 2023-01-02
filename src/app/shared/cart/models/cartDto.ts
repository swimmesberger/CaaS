import { DiscountDto } from './discountDto';
import { CartItemDto } from './cartItemDto';
import { CustomerDto } from './customerDto';
import { CouponDto } from './couponDto';


export interface CartDto {
    id?: string;
    shopId?: string;
    customer?: CustomerDto;
    items?: Array<CartItemDto> | null;
    coupons?: Array<CouponDto> | null;
    cartDiscounts?: Array<DiscountDto> | null;
    lastAccess?: string;
    totalPrice?: number;
    concurrencyToken?: string | null;
}

