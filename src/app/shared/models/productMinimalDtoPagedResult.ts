import { ParsedPaginationToken } from './parsedPaginationToken';
import { ProductMinimalDto } from './productMinimalDto';


export interface ProductMinimalDtoPagedResult {
    items?: Array<ProductMinimalDto> | null;
    totalCount?: number;
    totalPages?: number;
    firstPage?: ParsedPaginationToken;
    previousPage?: ParsedPaginationToken;
    nextPage?: ParsedPaginationToken;
    lastPage?: ParsedPaginationToken;
}

