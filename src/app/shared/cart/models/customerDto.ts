export interface CustomerDto {
    id?: string;
    name?: string | null;
    shopId?: string;
    eMail?: string | null;
    concurrencyToken?: string | null;
}

