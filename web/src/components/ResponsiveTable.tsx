import type { ReactNode } from 'react'
import { Card } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'

export type Column<T> = {
  header: string
  /** Leave the column out of the mobile card (it still shows from md up). */
  hideOnMobile?: boolean
  cell: (item: T) => ReactNode
}

type ResponsiveTableProps<T> = {
  items: readonly T[]
  getKey: (item: T) => string
  /** The first column is the card title on mobile. */
  columns: readonly Column<T>[]
  actions?: (item: T) => ReactNode
}

/** A table from md up, a list of cards on mobile. */
export function ResponsiveTable<T>({ items, getKey, columns, actions }: ResponsiveTableProps<T>) {
  const [titleColumn, ...otherColumns] = columns
  const detailColumns = otherColumns.filter((column) => !column.hideOnMobile)

  return (
    <>
      <div className="hidden md:block">
        <Table>
          <TableHeader>
            <TableRow>
              {columns.map((column, index) => (
                <TableHead key={index}>{column.header}</TableHead>
              ))}
              {actions ? (
                <TableHead>
                  <span className="sr-only">Actions</span>
                </TableHead>
              ) : null}
            </TableRow>
          </TableHeader>
          <TableBody>
            {items.map((item) => (
              <TableRow key={getKey(item)}>
                {columns.map((column, index) => (
                  <TableCell key={index}>{column.cell(item)}</TableCell>
                ))}
                {actions ? (
                  <TableCell>
                    <div className="flex justify-end gap-2">{actions(item)}</div>
                  </TableCell>
                ) : null}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <ul className="flex flex-col gap-3 md:hidden">
        {items.map((item) => (
          <li key={getKey(item)}>
            <Card>
              <div className="font-semibold break-words">{titleColumn?.cell(item)}</div>
              <dl className="flex flex-col gap-2 text-sm">
                {detailColumns.map((column, index) => (
                  <div key={index} className="flex flex-col gap-0.5">
                    <dt className="text-muted-foreground">{column.header}</dt>
                    <dd className="break-words">{column.cell(item)}</dd>
                  </div>
                ))}
              </dl>
              {actions ? <div className="flex flex-wrap gap-2">{actions(item)}</div> : null}
            </Card>
          </li>
        ))}
      </ul>
    </>
  )
}
