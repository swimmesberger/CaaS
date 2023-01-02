export interface ProductDetailDto {
    id?: string;
    shopId?: string;
    name?: string | null;
    description?: string | null;
    imageSrc?: string | null;
    price?: number;
    salePrice?: number | null;
}

