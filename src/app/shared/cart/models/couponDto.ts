export interface CouponDto {
    id?: string;
    shopId?: string;
    value?: number;
    orderId?: string | null;
    cartId?: string | null;
    customerId?: string | null;
    concurrencyToken?: string | null;
}

