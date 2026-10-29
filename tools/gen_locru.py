"""Генерирует LocRu.cs из словаря loc_ru.txt (строки вида English|Русский, # — комментарий)."""
import sys

src, dst = sys.argv[1], sys.argv[2]
pairs = []
for line in open(src, encoding='utf-8'):
    line = line.rstrip('\n')
    if not line.strip() or line.startswith('#'):
        continue
    en, ru = line.split('|', 1)
    pairs.append((en, ru))


def esc(s):
    return s.replace('\\', '\\\\').replace('"', '\\"')


out = ['using System.Collections.Generic;', '', 'namespace CombatSandbox.Demo;', '', 'internal static class LocRu', '{',
       '\tpublic static IEnumerable<(string en, string ru)> Pairs()', '\t{']
out += [f'\t\tyield return ("{esc(en)}", "{esc(ru)}");' for en, ru in pairs]
out += ['\t}', '}', '']
open(dst, 'w', encoding='utf-8').write('\n'.join(out))
