export interface DiscountDto {
    id?: string;
    discountName?: string | null;
    discountValue?: number;
    shopId?: string;
    parentId?: string;
    concurrencyToken?: string | null;
}

