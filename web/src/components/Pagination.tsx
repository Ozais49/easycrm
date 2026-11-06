import { Button } from '@/components/ui/button'

type PaginationProps = {
  page: number
  pageSize: number
  total: number
  onPageChange: (page: number) => void
}

export function Pagination({ page, pageSize, total, onPageChange }: PaginationProps) {
  const pageCount = Math.max(1, Math.ceil(total / pageSize))
  if (pageCount === 1) return null

  return (
    <div className="flex items-center justify-between gap-2">
      <Button variant="outline" disabled={page <= 1} onClick={() => onPageChange(page - 1)}>
        Previous
      </Button>
      <span className="text-muted-foreground text-sm">
        Page {page} of {pageCount}
      </span>
      <Button variant="outline" disabled={page >= pageCount} onClick={() => onPageChange(page + 1)}>
        Next
      </Button>
    </div>
  )
}
